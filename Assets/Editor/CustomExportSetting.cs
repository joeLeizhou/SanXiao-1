using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using LuaInterface.Editor;
using Orcas.Core;
using Orcas.Core.Tools;
using Orcas.Data;
using Orcas.Game.Common;
using Orcas.Lua.Core;
using Orcas.Networking;
using Orcas.Networking.NetClient;
using Orcas.Resources;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public class CustomExportSetting
{
    static CustomExportSetting()
	{
		ExportSettings.outputDir   = Application.dataPath + "/Scripts/CSharp/ToLua/";
		ExportSettings.generateDir = Application.dataPath + "/Scripts/CSharp/ToLua/Generate/";
		ExportSettings.baseTypeDir = Application.dataPath + "/Scripts/CSharp/ToLua/BaseType/";

		ExportSettings.customTypeList.AddRange(new List<ToLuaMenu.BindType>()
		{
			// ExportSettings._GT(typeof(MonoBehaviour)),
			ExportSettings._GT(typeof(Text)),
			ExportSettings._GT(typeof(Button)),
			ExportSettings._GT(typeof(BaseMeshEffect)),
			ExportSettings._GT(typeof(ContentSizeFitter)),
			ExportSettings._GT(typeof(ContentSizeFitter.FitMode)),
			ExportSettings._GT(typeof(Dropdown)),
			ExportSettings._GT(typeof(Dropdown.OptionData)),
			ExportSettings._GT(typeof(InputField)),
			ExportSettings._GT(typeof(InputField.ContentType)),
			ExportSettings._GT(typeof(RawImage)),
			ExportSettings._GT(typeof(SceneManager)),
			ExportSettings._GT(typeof(Image)),
			ExportSettings._GT(typeof(Toggle)),
			ExportSettings._GT(typeof(RectTransform)),
			ExportSettings._GT(typeof(RectTransformUtility)),
			ExportSettings._GT(typeof(Rect)),
			ExportSettings._GT(typeof(LayoutElement)),
			ExportSettings._GT(typeof(Slider)),
			ExportSettings._GT(typeof(CanvasGroup)),
			ExportSettings._GT(typeof(GridLayoutGroup)),
			ExportSettings._GT(typeof(Outline)),
			ExportSettings._GT(typeof(Shadow)),
			ExportSettings._GT(typeof(ScrollRect)),
			ExportSettings._GT(typeof(Scrollbar)),
			ExportSettings._GT(typeof(PointerEventData)),
			ExportSettings._GT(typeof(Canvas)),
			ExportSettings._GT(typeof(CanvasScaler)),
			ExportSettings._GT(typeof(EventSystem)),
			ExportSettings._GT(typeof(GraphicRaycaster)),
			ExportSettings._GT(typeof(Selectable.Transition)),
			ExportSettings._GT(typeof(Color)),
			ExportSettings._GT(typeof(Input)),
			ExportSettings._GT(typeof(TouchPhase)),
			ExportSettings._GT(typeof(Vector2Int)),
			ExportSettings._GT(typeof(Vector3Int)),
			ExportSettings._GT(typeof(Array)),
			ExportSettings._GT(typeof(ParticleSystem)),
			ExportSettings._GT(typeof(ParticleSystemRenderer)),
			ExportSettings._GT(typeof(ParticleSystem.MainModule)),
			ExportSettings._GT(typeof(ParticleSystem.MinMaxCurve)),
			ExportSettings._GT(typeof(ParticleSystem.EmissionModule)),
			ExportSettings._GT(typeof(Animator)),
			ExportSettings._GT(typeof(Animation)),
			ExportSettings._GT(typeof(TextMeshPro)),
			ExportSettings._GT(typeof(TextMeshProUGUI)),
			ExportSettings._GT(typeof(TMP_Text)),
			ExportSettings._GT(typeof(LayoutRebuilder)),
			ExportSettings._GT(typeof(SpriteRenderer)),
			ExportSettings._GT(typeof(Bounds)),
			ExportSettings._GT(typeof(Physics2D)),
			ExportSettings._GT(typeof(RaycastHit2D)),
			ExportSettings._GT(typeof(LoadSceneMode)),
			ExportSettings._GT(typeof(Collider2D)),
			//DOTween
            ExportSettings._GT(typeof(DG.Tweening.DOTween)),
            ExportSettings._GT(typeof(DG.Tweening.DOVirtual)),
            ExportSettings._GT(typeof(DG.Tweening.Tween)).SetBaseType(typeof(System.Object)).AddExtendType(typeof(DG.Tweening.TweenExtensions)),
            ExportSettings._GT(typeof(DG.Tweening.Sequence)).AddExtendType(typeof(DG.Tweening.TweenSettingsExtensions)),
            ExportSettings._GT(typeof(DG.Tweening.Tweener)).AddExtendType(typeof(DG.Tweening.TweenSettingsExtensions)),
            ExportSettings._GT(typeof(DG.Tweening.LoopType)),
            ExportSettings._GT(typeof(DG.Tweening.PathMode)),
            ExportSettings._GT(typeof(DG.Tweening.PathType)),
            ExportSettings._GT(typeof(DG.Tweening.RotateMode)),
            ExportSettings._GT(typeof(DG.Tweening.Ease)),
            ExportSettings._GT(typeof(Component)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
            ExportSettings._GT(typeof(Transform)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        //    ExportSettings._GT(typeof(Light)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
            ExportSettings._GT(typeof(Material)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
      //      ExportSettings._GT(typeof(Rigidbody)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
            ExportSettings._GT(typeof(Camera)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
            ExportSettings._GT(typeof(AudioSource)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
      //      ExportSettings._GT(typeof(LineRenderer)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        //    ExportSettings._GT(typeof(TrailRenderer)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
            ExportSettings._GT(typeof(Text)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions46)),
            ExportSettings._GT(typeof(RawImage)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions46)),
            ExportSettings._GT(typeof(Image)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions46)),
            ExportSettings._GT(typeof(RectTransform)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions46)),
            ExportSettings._GT(typeof(CanvasGroup)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions46)),

            ExportSettings._GT(typeof(UIEventManager)),
			ExportSettings._GT(typeof(UIDragManager)),
			ExportSettings._GT(typeof(UIDepthControl)),
			ExportSettings._GT(typeof(GameManager)),
			ExportSettings._GT(typeof(StorageManager)),
			ExportSettings._GT(typeof(Storage)),
			ExportSettings._GT(typeof(Map)),
			ExportSettings._GT(typeof(PlayerPrefsManager)),
			ExportSettings._GT(typeof(Progress.Item)),
			ExportSettings._GT(typeof(CLanguageManager)),
			ExportSettings._GT(typeof(GameClientManager)),
			ExportSettings._GT(typeof(NetClient)),
			ExportSettings._GT(typeof(ProtocolFactory)),
			ExportSettings._GT(typeof(SystemInfoHelper)),
			ExportSettings._GT(typeof(ResourceLoader)),
			ExportSettings._GT(typeof(SoundLoader)),
			ExportSettings._GT(typeof(MonoBehaviour)),
		});
		ToLuaMenu.AutoCheck();
	}
}
