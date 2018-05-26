/*
 * Copyright (c) 2017, Zimaya <https://github.com/Zimaya>
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using Partiality.Modloader;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace StoreDescriptions
{
	class StoreDescriptions : PartialityMod
	{
		public override void Init()
		{
			base.Init();
			ModID = "StoreDescriptions";
		}

		public override void OnLoad()
		{
			base.OnLoad();
			On.SkillStoreItem.Initialize += SkillStoreItemStart;
			On.ItemStoreItem.Start += ItemStoreItemOnStart;
			On.StoreItem.Update += StoreItemOnUpdate;
		}

		private void StoreItemOnUpdate(On.StoreItem.orig_Update orig, StoreItem self)
		{
			orig(self);

			var text = self.itemText;
			bool testbool = text.enabled;

			if (self.itemText.transform.GetComponentsInChildren<Text>().Length == 1)
				return;

			var child = self.itemText.transform.GetComponentsInChildren<Text>()[1];
			if (child.enabled == testbool)
				return;

			child.enabled = testbool;
		}

		private void ItemStoreItemOnStart(On.ItemStoreItem.orig_Start orig, ItemStoreItem item)
		{
			orig(item);
			Text text = CreateTextObject(item);
			text.text = TextManager.GetItemDescription(item.ID);
		}

		public void SkillStoreItemStart(On.SkillStoreItem.orig_Initialize orig, SkillStoreItem item)
		{
			orig(item);
			Text text = CreateTextObject(item);
			text.text = TextManager.GetSkillDescription(item.skillID);
		}

		private static Text CreateTextObject(StoreItem item)
		{
			var pos = item.itemText.transform.position;

			var pricePrefab = ChaosBundle.Get("Assets/Prefabs/Environment/Store/PriceMarker.prefab").transform.Find("Canvas");
			var go = Object.Instantiate(pricePrefab, pos, Quaternion.identity);
			go.SetParent(item.itemText.transform);
			var textObj = go.GetComponentInChildren<Text>();
			var textRect = textObj.GetComponent<RectTransform>();

			go.localScale = Vector3.one / 3f;
			go.localPosition = Vector3.zero - Vector3.up/2;
			textRect.sizeDelta = new Vector2(40, 1);
			textObj.transform.localPosition = Vector3.zero;

			textObj.supportRichText = true;
			textObj.horizontalOverflow = HorizontalWrapMode.Wrap;
			textObj.verticalOverflow = VerticalWrapMode.Overflow;
			textObj.alignment = TextAnchor.UpperCenter;
			textObj.fontSize = 3;
			textObj.color = Color.white;

			var outline = textObj.gameObject.AddComponent<Outline>();
			outline.effectDistance = Vector2.one / 6;
			outline.effectColor = Color.black;

			return textObj;
		}
	}
}
