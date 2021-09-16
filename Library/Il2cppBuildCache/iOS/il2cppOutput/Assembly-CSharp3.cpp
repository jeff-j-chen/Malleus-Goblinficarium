#include "pch-cpp.hpp"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <limits>
#include <stdint.h>


template <typename T1>
struct VirtualActionInvoker1
{
	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		void* params[1] = { &p1 };
		invokeData.method->invoker_method(invokeData.methodPtr, invokeData.method, obj, params, params[0]);
	}
};
template <typename T1>
struct VirtualActionInvoker1<T1*>
{
	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1* p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		void* params[1] = { p1 };
		invokeData.method->invoker_method(invokeData.methodPtr, invokeData.method, obj, params, params[0]);
	}
};
template <typename R>
struct VirtualFuncInvoker0
{
	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		R ret;
		invokeData.method->invoker_method(invokeData.methodPtr, invokeData.method, obj, NULL, &ret);
		return ret;
	}
};
template <typename T1>
struct InvokerCallActionInvoker1;
template <typename T1>
struct InvokerCallActionInvoker1<T1*>
{
	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1* p1)
	{
		void* params[1] = { p1 };
		method->invoker_method(il2cpp_codegen_get_method_pointer(method), method, obj, params, params[0]);
	}
};

// System.Action`1<System.Int32>
struct Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404;
// System.Action`1<TMPro.TMP_TextInfo>
struct Action_1_tB93AB717F9D419A1BEC832FF76E74EAA32184CC1;
// System.Action`2<System.Int32,System.Int32>
struct Action_2_tD7438462601D3939500ED67463331FE00CFFBDB8;
// System.Collections.Generic.Dictionary`2<System.Int32,System.Int32>
struct Dictionary_2_tABE19B9C5C52F1DE14F0D3287B2696E7D7419180;
// System.Collections.Generic.Dictionary`2<System.Single,UnityEngine.WaitForSeconds>
struct Dictionary_2_t5826E39B66190EFF2D3EA81361D5FC4BB8D6B212;
// System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.List`1<Dice>>
struct Dictionary_2_t71DC4111D9530C502ACEA7A17068D6DC7AF41689;
// System.Collections.Generic.Dictionary`2<System.String,System.Int32>
struct Dictionary_2_t5C8F46F5D57502270DD9E1DA8303B23C7FE85588;
// System.Collections.Generic.Dictionary`2<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType,Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>
struct Dictionary_2_t5C32AF17A5801FB3109E5B0E622BA8402A04E08E;
// System.Func`3<System.Int32,System.String,TMPro.TMP_FontAsset>
struct Func_3_tC721DF8CDD07ED66A4833A19A2ED2302608C906C;
// System.Func`3<System.Int32,System.String,TMPro.TMP_SpriteAsset>
struct Func_3_t6F6D9932638EA1A5A45303C6626C818C25D164E5;
// System.Collections.Generic.IEnumerable`1<Dice>
struct IEnumerable_1_t2A8927D3C7D1C68974E5A362856FA17A955EDF19;
// System.Collections.Generic.IEnumerable`1<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>
struct IEnumerable_1_t29E7244AE33B71FA0981E50D5BC73B7938F35C66;
// System.Collections.Generic.IEqualityComparer`1<System.Single>
struct IEqualityComparer_1_t0F7348B7C3DBAC2DFD60DA8607A8BCD442B3E713;
// System.Collections.Generic.IEqualityComparer`1<System.String>
struct IEqualityComparer_1_tAE94C8F24AD5B94D4EE85CA9FC59E3409D41CAF7;
// System.Collections.Generic.Dictionary`2/KeyCollection<System.Single,UnityEngine.WaitForSeconds>
struct KeyCollection_tD9E01F0D5662E23CB830A775ECEAC137B1182595;
// System.Collections.Generic.Dictionary`2/KeyCollection<System.String,System.Collections.Generic.List`1<Dice>>
struct KeyCollection_t8D3065FD70EC83CC2EB44BDE22DB56011A4C1846;
// System.Collections.Generic.List`1<UnityEngine.Color>
struct List_1_t242CDEAEC9C92000DA96982CDB9D592DDE2AADAF;
// System.Collections.Generic.List`1<Dice>
struct List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789;
// System.Collections.Generic.List`1<UnityEngine.GameObject>
struct List_1_tB951CE80B58D1BF9650862451D8DAD8C231F207B;
// System.Collections.Generic.List`1<UnityEngine.UI.Image>
struct List_1_tE6BB71ABF15905EFA2BE92C38A2716547AEADB19;
// System.Collections.Generic.List`1<System.String>
struct List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD;
// System.Collections.Generic.List`1<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>
struct List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A;
// UnityEngine.UI.CoroutineTween.TweenRunner`1<UnityEngine.UI.CoroutineTween.ColorTween>
struct TweenRunner_1_t5BB0582F926E75E2FE795492679A6CF55A4B4BC4;
// UnityEngine.Events.UnityAction`2<UnityEngine.SceneManagement.Scene,UnityEngine.SceneManagement.LoadSceneMode>
struct UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A;
// UnityEngine.Events.UnityAction`2<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType,Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>
struct UnityAction_2_t742C43FA6EAABE0458C753DFE15FDDFAE01EA73F;
// UnityEngine.Events.UnityEvent`1<UnityEngine.SpriteRenderer>
struct UnityEvent_1_t8ABE5544759145B8D7A09F1C54FFCB6907EDD56E;
// System.Collections.Generic.Dictionary`2/ValueCollection<System.Single,UnityEngine.WaitForSeconds>
struct ValueCollection_t7EEE94E5094B21A3BBABBE484A7A25DE1FD1729A;
// System.Collections.Generic.Dictionary`2/ValueCollection<System.String,System.Collections.Generic.List`1<Dice>>
struct ValueCollection_tB2C891982229C581FE1C925FA124E2C8D2AC1FF0;
// System.Collections.Generic.Dictionary`2/Entry<System.Single,UnityEngine.WaitForSeconds>[]
struct EntryU5BU5D_t62AEF587819B78CCFC47041BCDC4BC10ECC876DA;
// System.Collections.Generic.Dictionary`2/Entry<System.String,System.Collections.Generic.List`1<Dice>>[]
struct EntryU5BU5D_tDD93C97ED48EBC109C940090E59DBB26D0E4EFFC;
// TMPro.TMP_TextProcessingStack`1<System.Int32>[]
struct TMP_TextProcessingStack_1U5BU5D_t08293E0BB072311BB96170F351D1083BCA97B9B2;
// UnityEngine.AudioClip[]
struct AudioClipU5BU5D_t916722468F7EDCFA833318C35CD7D41097D75D31;
// System.Byte[]
struct ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031;
// System.Char[]
struct CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB;
// UnityEngine.Color32[]
struct Color32U5BU5D_t38116C3E91765C4C5726CE12C77FAD7F9F737259;
// System.Decimal[]
struct DecimalU5BU5D_t93BA0C88FA80728F73B792EE1A5199D0C060B615;
// System.Delegate[]
struct DelegateU5BU5D_tC5AB7E8F745616680F337909D3A8E6C722CDF771;
// Dice[]
struct DiceU5BU5D_t1164BC19F6E4DE976672A57E270EC84BEE96AE55;
// TMPro.FontWeight[]
struct FontWeightU5BU5D_t2A406B5BAB0DD0F06E7F1773DB062E4AF98067BA;
// UnityEngine.GameObject[]
struct GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF;
// TMPro.HighlightState[]
struct HighlightStateU5BU5D_tA878A0AF1F4F52882ACD29515AADC277EE135622;
// TMPro.HorizontalAlignmentOptions[]
struct HorizontalAlignmentOptionsU5BU5D_t4D185662282BFB910D8B9A8199E91578E9422658;
// System.Int32[]
struct Int32U5BU5D_t19C97395396A72ECAF310612F0760F165060314C;
// System.IntPtr[]
struct IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832;
// UnityEngine.Material[]
struct MaterialU5BU5D_t2B1D11C42DB07A4400C0535F92DBB87A2E346D3D;
// TMPro.MaterialReference[]
struct MaterialReferenceU5BU5D_t7491D335AB3E3E13CE9C0F5E931F396F6A02E1F2;
// TMPro.RichTextTagAttribute[]
struct RichTextTagAttributeU5BU5D_t5816316EFD8F59DBC30B9F88E15828C564E47B6D;
// System.Single[]
struct SingleU5BU5D_t89DEFE97BCEDB5857010E79ECE0F52CF6E93B87C;
// System.Diagnostics.StackTrace[]
struct StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF;
// System.String[]
struct StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248;
// TMPro.TMP_CharacterInfo[]
struct TMP_CharacterInfoU5BU5D_t297D56FCF66DAA99D8FEA7C30F9F3926902C5B99;
// TMPro.TMP_ColorGradient[]
struct TMP_ColorGradientU5BU5D_t2F65E8C42F268DFF33BB1392D94BCF5B5087308A;
// TMPro.TMP_SubMeshUI[]
struct TMP_SubMeshUIU5BU5D_tC77B263183A59A75345C26152457207EAC3BBF29;
// System.UInt32[]
struct UInt32U5BU5D_t02FBD658AD156A17574ECE6106CF1FBFCC9807FA;
// UnityEngine.Vector2[]
struct Vector2U5BU5D_tFEBBC94BCC6C9C88277BA04047D2B3FDB6ED7FDA;
// UnityEngine.Vector3[]
struct Vector3U5BU5D_tFF1859CCE176131B909E2044F76443064254679C;
// UnityEngine.WaitForSeconds[]
struct WaitForSecondsU5BU5D_t2A9038ECB6E643745AEF2AD9A4F7FFD3D272186E;
// TMPro.WordWrapState[]
struct WordWrapStateU5BU5D_t473D59C9DBCC949CE72EF1EB471CBA152A6CEAC9;
// Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType[]
struct __Il2CppFullySharedGenericTypeU5BU5D_tCAB6D060972DD49223A834B7EEFEB9FE2D003BEC;
// TMPro.TMP_Text/UnicodeChar[]
struct UnicodeCharU5BU5D_t67F27D09F8EB28D2C42DFF16FE60054F157012F5;
// UnityEngine.Animator
struct Animator_t8A52E42AE54F76681838FE9E632683EF3952E883;
// Arrow
struct Arrow_t7048A6830A9F76E0448BBD44FD9C3C00BC138DBF;
// UnityEngine.AudioSource
struct AudioSource_t871AC2272F896738252F04EE949AEF5B241D3299;
// BackToMenu
struct BackToMenu_tD8E3B8BA9822D00CBD4BA804F41B32438B39E4BC;
// UnityEngine.Canvas
struct Canvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26;
// UnityEngine.CanvasGroup
struct CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094;
// UnityEngine.CanvasRenderer
struct CanvasRenderer_tAB9A55A976C4E3B2B37D0CE5616E5685A8B43860;
// CharacterSelector
struct CharacterSelector_t3FDA33FAF8CF21DF9FC1E34A558DA73D2FACB64E;
// Colors
struct Colors_t4FBC8F9BC3173CFB15C6164CC275EEBAC6E0E973;
// UnityEngine.Component
struct Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3;
// UnityEngine.Coroutine
struct Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B;
// System.DelegateData
struct DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E;
// Dice
struct Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A;
// DiceSummoner
struct DiceSummoner_tA51E21ADF511DD948DD0B44C50A73CF2F8C29DBC;
// Enemy
struct Enemy_t10DB314C96B1CE78B8D967CD3B39F05126409BBB;
// Fader
struct Fader_t4082384C0679E40CABDB8F1A51E0246989537D24;
// UnityEngine.GameObject
struct GameObject_t76FEDD663AB33C991A9C9A23129337651094216F;
// HighlightCalculator
struct HighlightCalculator_t9A040BB70BE3C30320C9B31D3A60280A9D27E9B6;
// System.Collections.IDictionary
struct IDictionary_t6D03155AF1FA9083817AA5B6AD7DEEACC26AB220;
// System.Collections.IEnumerator
struct IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA;
// TMPro.ITextPreprocessor
struct ITextPreprocessor_tDBB49C8B68D7B80E8D233B9D9666C43981EFAAB9;
// UnityEngine.UI.Image
struct Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E;
// ItemManager
struct ItemManager_t396CBB91EABD1E73BA16A4DBE2E89AC1601FB83E;
// UnityEngine.UI.LayoutElement
struct LayoutElement_tB1F24CC11AF4AA87015C8D8EE06D22349C5BF40A;
// LevelManager
struct LevelManager_t8405886BBC5A0ACBB1CC210E25D5DA1C72D16530;
// UnityEngine.Material
struct Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3;
// MenuButton
struct MenuButton_t5FBBD57E4378A9FCDB747615454A243C197C42A2;
// MenuIcon
struct MenuIcon_tC1941FC04C0252157C37212443EB28203FBE0EA9;
// UnityEngine.Mesh
struct Mesh_t6D9C539763A09BC2B12AEAEF36F6DFFC98AE63D4;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// UnityEngine.MonoBehaviour
struct MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71;
// Music
struct Music_t95D6293158A4741467B5F53F0E61597A72226ECD;
// System.NotSupportedException
struct NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A;
// UnityEngine.Object
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C;
// Player
struct Player_tF98BD09D3495D2FF1922E5D34866AEAC6AE2DF74;
// UnityEngine.UI.RectMask2D
struct RectMask2D_tACF92BE999C791A665BD1ADEABF5BCEB82846670;
// UnityEngine.RectTransform
struct RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6;
// Scripts
struct Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C;
// SoundManager
struct SoundManager_tCA2CCAC5CDF1BA10E525C01C8D1D0DFAC9BE3734;
// UnityEngine.Sprite
struct Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99;
// UnityEngine.SpriteRenderer
struct SpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B;
// StatSummoner
struct StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539;
// Statistics
struct Statistics_t189B79C95098317A18A30BF4120F43A59ABB6431;
// System.String
struct String_t;
// TMPro.TMP_Character
struct TMP_Character_t7D37A55EF1A9FF6D0BFE6D50E86A00F80E7FAF35;
// TMPro.TMP_ColorGradient
struct TMP_ColorGradient_t17B51752B4E9499A1FF7D875DCEC1D15A0F4AEBB;
// TMPro.TMP_FontAsset
struct TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160;
// TMPro.TMP_SpriteAnimator
struct TMP_SpriteAnimator_t2E0F016A61CA343E3222FF51E7CF0E53F9F256E4;
// TMPro.TMP_SpriteAsset
struct TMP_SpriteAsset_t81F779E6F705CE190DC0D1F93A954CB8B1774B39;
// TMPro.TMP_Style
struct TMP_Style_tA9E5B1B35EBFE24EF980CEA03251B638282E120C;
// TMPro.TMP_StyleSheet
struct TMP_StyleSheet_t70C71699F5CB2D855C361DBB78A44C901236C859;
// TMPro.TMP_TextElement
struct TMP_TextElement_t262A55214F712D4274485ABE5676E5254B84D0A5;
// TMPro.TMP_TextInfo
struct TMP_TextInfo_t09A8E906329422C3F0C059876801DD695B8D524D;
// TMPro.TextMeshProUGUI
struct TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957;
// UnityEngine.Texture2D
struct Texture2D_tE6505BC111DD8A424A9DBE8E05D7D09E11FFFCF4;
// TombstoneData
struct TombstoneData_t98D1E2F7C78F45B3AE4D2A37D2FF480FFC3F7CC5;
// UnityEngine.Transform
struct Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1;
// TurnManager
struct TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41;
// Tutorial
struct Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4;
// UnityEngine.Events.UnityAction
struct UnityAction_t11A1F3B953B365C072A5DCC32677EE1796A962A7;
// UnityEngine.UI.VertexHelper
struct VertexHelper_tB905FCB02AE67CBEE5F265FE37A5938FC5D136FE;
// System.Void
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915;
// UnityEngine.WaitForSeconds
struct WaitForSeconds_tF179DF251655B8DF044952E70A60DF4B358A3DD3;
// UnityEngine.Canvas/WillRenderCanvases
struct WillRenderCanvases_tA4A6E66DBA797DCB45B995DBA449A9D1D80D0FBC;
// Fader/<FadeIt>d__13
struct U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099;
// UnityEngine.UI.MaskableGraphic/CullStateChangedEvent
struct CullStateChangedEvent_t6073CD0D951EC1256BF74B8F9107D68FC89B99B8;
// TurnManager/<RemoveDice>d__41
struct U3CRemoveDiceU3Ed__41_tF3DAB6BC24EFB53E73F50C0E34AB6F2644CC077A;
// Tutorial/<TextAnimation>d__13
struct U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02;
// Tutorial/<TextAnimation>d__14
struct U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8;

IL2CPP_EXTERN_C RuntimeClass* Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* GameObject_t76FEDD663AB33C991A9C9A23129337651094216F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Initiate_t4923CFF950DA8CE1826343BD14BE825EDDD5AE2C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* RuntimeObject_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral0DA8B99A80456CF0447EE4E9C8076CC92116C70F;
IL2CPP_EXTERN_C String_t* _stringLiteral117FBC2BBE50A7C98480590D08F348D5A9097BBD;
IL2CPP_EXTERN_C String_t* _stringLiteral1205791EEFC8465A8944A7B058812B718E7F0909;
IL2CPP_EXTERN_C String_t* _stringLiteral1229F9FF79387730D4FE96408933191647B39278;
IL2CPP_EXTERN_C String_t* _stringLiteral267C2561791B8066965291F6BF4AC3AD56744BBC;
IL2CPP_EXTERN_C String_t* _stringLiteral630D82DDB91BD0A38D5F5BE931D8D7433736DCDC;
IL2CPP_EXTERN_C String_t* _stringLiteral73D105A88284D23B2E558768E82867E4EDBEE9C0;
IL2CPP_EXTERN_C String_t* _stringLiteral83A93055434B3DEFBA1FDCD5E3FE2A6CFFCB4D34;
IL2CPP_EXTERN_C String_t* _stringLiteral89BF96A86F8214B4D8D2B1FAC0423C4D55046B94;
IL2CPP_EXTERN_C String_t* _stringLiteral89E8BF424EDFA2DED494C2FF771626C991B4968F;
IL2CPP_EXTERN_C String_t* _stringLiteral8E55E46B8E8F02700ABAE1E9030A9CEEE3D96575;
IL2CPP_EXTERN_C String_t* _stringLiteral988F93A95ABFE53FF853FD26CE410B82A3E23BE3;
IL2CPP_EXTERN_C String_t* _stringLiteral9C04205D437A60FD17FE5555B3502042C6A436F3;
IL2CPP_EXTERN_C String_t* _stringLiteralA199A05EF4B94C954675DBB8F451DCE18F37C004;
IL2CPP_EXTERN_C String_t* _stringLiteralA5BF6288DB720FF5F6257F5F0961A932300AA7AC;
IL2CPP_EXTERN_C String_t* _stringLiteralAB6C48F356D6E8BD57F319FACD990F998C0B878B;
IL2CPP_EXTERN_C String_t* _stringLiteralB71034B75D3E4B918B899344D72D11E843F82F1D;
IL2CPP_EXTERN_C String_t* _stringLiteralBADE01FA72995907D8CBB52ED0948028121B2A9E;
IL2CPP_EXTERN_C String_t* _stringLiteralBD6390C3D498E36B80DABABBC961431C400B0719;
IL2CPP_EXTERN_C String_t* _stringLiteralD3E06A4C4A311F489EB979283DBDC7CBB6CB9CF0;
IL2CPP_EXTERN_C String_t* _stringLiteralD4E298E8840B5A18F01203A40A9ABA9D222F56FE;
IL2CPP_EXTERN_C String_t* _stringLiteralD8431B5D5BBDD13458B95AC3252777089DFF7F0A;
IL2CPP_EXTERN_C String_t* _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709;
IL2CPP_EXTERN_C String_t* _stringLiteralE12AF4099C3E5EC3FE1AB438E75CC43EEA1E65C6;
IL2CPP_EXTERN_C String_t* _stringLiteralE1A596C859D35600B5AA7D4F585C3A76013BA55D;
IL2CPP_EXTERN_C String_t* _stringLiteralE9923573DCC65C12B03FC7B779BF1A80B345C19E;
IL2CPP_EXTERN_C String_t* _stringLiteralEB6A7D6D86E053EAEC72B09FACC475E0F45D4E42;
IL2CPP_EXTERN_C String_t* _stringLiteralECF501476651A1D856C0760EB5C4004165FDCD87;
IL2CPP_EXTERN_C String_t* _stringLiteralF330F1A44EDD893918E4984ECCD79A10D13E93EF;
IL2CPP_EXTERN_C String_t* _stringLiteralF680D63E130D5BE4EB8EAF5C9C69BFD423331BD0;
IL2CPP_EXTERN_C String_t* _stringLiteralFA9A9682704E0A907999201CAF9437D0F2E581AB;
IL2CPP_EXTERN_C String_t* _stringLiteralFE6476BE639D4C086B597DB56C0A679D5B634C5A;
IL2CPP_EXTERN_C const RuntimeMethod* Component_GetComponentInChildren_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_m22ACF33DC0AB281D8B1E18650516D0765006FE66_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Component_GetComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_mA3B0428368982ED39ADEBB220EE67D1E99D8B2D2_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Component_GetComponent_TisDice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A_m91F6B03AAEFF32E02B3AC36981E9D444FD235085_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Component_GetComponent_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_mE74EE63C85A63FC34DCFC631BC229207B420BC79_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Component_GetComponent_TisSpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B_m6181F10C09FC1650DAE0EF2308D344A2F170AA45_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_get_Item_mBEB955470239D94BCB11C23D263104688CC2595B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_get_Item_mF08AF6105F6A747B98757834E9BE7FE975620925_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerable_ToList_TisDice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A_m34AE9657FC95F123345470E6C2B585FC6CD58C19_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_Dispose_m183F16152D45183DC6CE1A195F14D3EB174A560D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_MoveNext_m8E6AC5FCF5864D5C53C3838A777B86D2A9F5FDF7_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_get_Current_m789E5F6B4BF5820DBED7D75DB908232A6B38C182_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Fader_OnLevelFinishedLoading_m9904937D6C2056B6AB61465D48D3818119985E2B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_AddComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_m1C004B58918BA839B892637D46D95AF04D69DADA_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_AddComponent_TisCanvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26_m13C85FD585C0679530F8B35D0B39D965702FD0F5_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_AddComponent_TisFader_t4082384C0679E40CABDB8F1A51E0246989537D24_m05AEC75245A2C82F9D47A618CC0DE93E72102C3B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_AddComponent_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_mA327C9E1CA12BC531D587E7567F2067B96E6B6A0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_GetComponent_TisFader_t4082384C0679E40CABDB8F1A51E0246989537D24_mC0661A39B823BACE89B865B139AD471E8E5A3B18_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_GetEnumerator_m56D864FDB01A87F36472DFA6F97147CDA63DE0B0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_Remove_m6ECEFB43E9BD1D878B805AA46E41073EEA8F2C6F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_Remove_mCCE85D4D5326536C4B214C73D07030F4CCD18485_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_get_Count_m4C37ED2D928D63B80F55AF434730C2D64EEB9F22_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_get_Count_mB63183A9151F4345A9DD444A7CBE0D6E03F77C7C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_get_Count_mD2E0CF4E337A946034B00E365D8B3B66445B4044_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_get_Item_m21AEC50E791371101DC22ABCF96A2E46800811F8_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Object_FindObjectOfType_TisScripts_tA1F67F81394769A4746B08F203BBD942A423AF5C_mD4A9D0F71D72CAEF1369857214457214E426E335_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CFadeItU3Ed__13_System_Collections_IEnumerator_Reset_m5A4CF16FF8C3EE5FF2A0397BBDE87E85BB948611_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CRemoveDiceU3Ed__41_System_Collections_IEnumerator_Reset_mC09053FA9A21B90ED860BFE6AD8A76F2772507C0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CTextAnimationU3Ed__13_System_Collections_IEnumerator_Reset_m241E39AA3DC8025128EE588DABBA361C4F5BF389_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CTextAnimationU3Ed__14_System_Collections_IEnumerator_Reset_mCE1FC3BE7799586C509C11FA2AF2C109B46A280A_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* UnityAction_2__ctor_m0E0C01B7056EB1CB1E6C6F4FC457EBCA3F6B0041_RuntimeMethod_var;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;

struct __Il2CppFullySharedGenericTypeU5BU5D_tCAB6D060972DD49223A834B7EEFEB9FE2D003BEC;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// System.Collections.Generic.Dictionary`2<System.Single,UnityEngine.WaitForSeconds>
struct Dictionary_2_t5826E39B66190EFF2D3EA81361D5FC4BB8D6B212  : public RuntimeObject
{
	// System.Int32[] System.Collections.Generic.Dictionary`2::_buckets
	Int32U5BU5D_t19C97395396A72ECAF310612F0760F165060314C* ____buckets_0;
	// System.Collections.Generic.Dictionary`2/Entry<TKey,TValue>[] System.Collections.Generic.Dictionary`2::_entries
	EntryU5BU5D_t62AEF587819B78CCFC47041BCDC4BC10ECC876DA* ____entries_1;
	// System.Int32 System.Collections.Generic.Dictionary`2::_count
	int32_t ____count_2;
	// System.Int32 System.Collections.Generic.Dictionary`2::_freeList
	int32_t ____freeList_3;
	// System.Int32 System.Collections.Generic.Dictionary`2::_freeCount
	int32_t ____freeCount_4;
	// System.Int32 System.Collections.Generic.Dictionary`2::_version
	int32_t ____version_5;
	// System.Collections.Generic.IEqualityComparer`1<TKey> System.Collections.Generic.Dictionary`2::_comparer
	RuntimeObject* ____comparer_6;
	// System.Collections.Generic.Dictionary`2/KeyCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::_keys
	KeyCollection_tD9E01F0D5662E23CB830A775ECEAC137B1182595* ____keys_7;
	// System.Collections.Generic.Dictionary`2/ValueCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::_values
	ValueCollection_t7EEE94E5094B21A3BBABBE484A7A25DE1FD1729A* ____values_8;
	// System.Object System.Collections.Generic.Dictionary`2::_syncRoot
	RuntimeObject* ____syncRoot_9;
};

// System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.List`1<Dice>>
struct Dictionary_2_t71DC4111D9530C502ACEA7A17068D6DC7AF41689  : public RuntimeObject
{
	// System.Int32[] System.Collections.Generic.Dictionary`2::_buckets
	Int32U5BU5D_t19C97395396A72ECAF310612F0760F165060314C* ____buckets_0;
	// System.Collections.Generic.Dictionary`2/Entry<TKey,TValue>[] System.Collections.Generic.Dictionary`2::_entries
	EntryU5BU5D_tDD93C97ED48EBC109C940090E59DBB26D0E4EFFC* ____entries_1;
	// System.Int32 System.Collections.Generic.Dictionary`2::_count
	int32_t ____count_2;
	// System.Int32 System.Collections.Generic.Dictionary`2::_freeList
	int32_t ____freeList_3;
	// System.Int32 System.Collections.Generic.Dictionary`2::_freeCount
	int32_t ____freeCount_4;
	// System.Int32 System.Collections.Generic.Dictionary`2::_version
	int32_t ____version_5;
	// System.Collections.Generic.IEqualityComparer`1<TKey> System.Collections.Generic.Dictionary`2::_comparer
	RuntimeObject* ____comparer_6;
	// System.Collections.Generic.Dictionary`2/KeyCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::_keys
	KeyCollection_t8D3065FD70EC83CC2EB44BDE22DB56011A4C1846* ____keys_7;
	// System.Collections.Generic.Dictionary`2/ValueCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::_values
	ValueCollection_tB2C891982229C581FE1C925FA124E2C8D2AC1FF0* ____values_8;
	// System.Object System.Collections.Generic.Dictionary`2::_syncRoot
	RuntimeObject* ____syncRoot_9;
};

// System.Collections.Generic.List`1<Dice>
struct List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789  : public RuntimeObject
{
	// T[] System.Collections.Generic.List`1::_items
	DiceU5BU5D_t1164BC19F6E4DE976672A57E270EC84BEE96AE55* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject* ____syncRoot_4;
};

struct List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789_StaticFields
{
	// T[] System.Collections.Generic.List`1::s_emptyArray
	DiceU5BU5D_t1164BC19F6E4DE976672A57E270EC84BEE96AE55* ___s_emptyArray_5;
};

// System.Collections.Generic.List`1<UnityEngine.GameObject>
struct List_1_tB951CE80B58D1BF9650862451D8DAD8C231F207B  : public RuntimeObject
{
	// T[] System.Collections.Generic.List`1::_items
	GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject* ____syncRoot_4;
};

struct List_1_tB951CE80B58D1BF9650862451D8DAD8C231F207B_StaticFields
{
	// T[] System.Collections.Generic.List`1::s_emptyArray
	GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* ___s_emptyArray_5;
};

// System.Collections.Generic.List`1<System.String>
struct List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD  : public RuntimeObject
{
	// T[] System.Collections.Generic.List`1::_items
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject* ____syncRoot_4;
};

struct List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD_StaticFields
{
	// T[] System.Collections.Generic.List`1::s_emptyArray
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___s_emptyArray_5;
};

// System.Collections.Generic.List`1<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>
struct List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A  : public RuntimeObject
{
	// T[] System.Collections.Generic.List`1::_items
	__Il2CppFullySharedGenericTypeU5BU5D_tCAB6D060972DD49223A834B7EEFEB9FE2D003BEC* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject* ____syncRoot_4;
};

struct List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A_StaticFields
{
	// T[] System.Collections.Generic.List`1::s_emptyArray
	__Il2CppFullySharedGenericTypeU5BU5D_tCAB6D060972DD49223A834B7EEFEB9FE2D003BEC* ___s_emptyArray_5;
};
struct Il2CppArrayBounds;

// Initiate
struct Initiate_t4923CFF950DA8CE1826343BD14BE825EDDD5AE2C  : public RuntimeObject
{
};

struct Initiate_t4923CFF950DA8CE1826343BD14BE825EDDD5AE2C_StaticFields
{
	// System.Boolean Initiate::areWeFading
	bool ___areWeFading_0;
};

// System.String
struct String_t  : public RuntimeObject
{
	// System.Int32 System.String::_stringLength
	int32_t ____stringLength_4;
	// System.Char System.String::_firstChar
	Il2CppChar ____firstChar_5;
};

struct String_t_StaticFields
{
	// System.String System.String::Empty
	String_t* ___Empty_6;
};

// System.ValueType
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F  : public RuntimeObject
{
};
// Native definition for P/Invoke marshalling of System.ValueType
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.ValueType
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_com
{
};

// UnityEngine.YieldInstruction
struct YieldInstruction_tFCE35FD0907950EFEE9BC2890AC664E41C53728D  : public RuntimeObject
{
};
// Native definition for P/Invoke marshalling of UnityEngine.YieldInstruction
struct YieldInstruction_tFCE35FD0907950EFEE9BC2890AC664E41C53728D_marshaled_pinvoke
{
};
// Native definition for COM marshalling of UnityEngine.YieldInstruction
struct YieldInstruction_tFCE35FD0907950EFEE9BC2890AC664E41C53728D_marshaled_com
{
};

// Fader/<FadeIt>d__13
struct U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099  : public RuntimeObject
{
	// System.Int32 Fader/<FadeIt>d__13::<>1__state
	int32_t ___U3CU3E1__state_0;
	// System.Object Fader/<FadeIt>d__13::<>2__current
	RuntimeObject* ___U3CU3E2__current_1;
	// Fader Fader/<FadeIt>d__13::<>4__this
	Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* ___U3CU3E4__this_2;
	// System.Boolean Fader/<FadeIt>d__13::<hasFadedIn>5__2
	bool ___U3ChasFadedInU3E5__2_3;
};

// TurnManager/<RemoveDice>d__41
struct U3CRemoveDiceU3Ed__41_tF3DAB6BC24EFB53E73F50C0E34AB6F2644CC077A  : public RuntimeObject
{
	// System.Int32 TurnManager/<RemoveDice>d__41::<>1__state
	int32_t ___U3CU3E1__state_0;
	// System.Object TurnManager/<RemoveDice>d__41::<>2__current
	RuntimeObject* ___U3CU3E2__current_1;
	// System.String TurnManager/<RemoveDice>d__41::removeFrom
	String_t* ___removeFrom_2;
	// TurnManager TurnManager/<RemoveDice>d__41::<>4__this
	TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* ___U3CU3E4__this_3;
	// System.String TurnManager/<RemoveDice>d__41::diceType
	String_t* ___diceType_4;
	// System.Collections.Generic.List`1<Dice> TurnManager/<RemoveDice>d__41::<diceList>5__2
	List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* ___U3CdiceListU3E5__2_5;
};

// Tutorial/<TextAnimation>d__13
struct U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02  : public RuntimeObject
{
	// System.Int32 Tutorial/<TextAnimation>d__13::<>1__state
	int32_t ___U3CU3E1__state_0;
	// System.Object Tutorial/<TextAnimation>d__13::<>2__current
	RuntimeObject* ___U3CU3E2__current_1;
	// Tutorial Tutorial/<TextAnimation>d__13::<>4__this
	Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* ___U3CU3E4__this_2;
	// System.Int32 Tutorial/<TextAnimation>d__13::index
	int32_t ___index_3;
	// System.Int32 Tutorial/<TextAnimation>d__13::<i>5__2
	int32_t ___U3CiU3E5__2_4;
};

// Tutorial/<TextAnimation>d__14
struct U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8  : public RuntimeObject
{
	// System.Int32 Tutorial/<TextAnimation>d__14::<>1__state
	int32_t ___U3CU3E1__state_0;
	// System.Object Tutorial/<TextAnimation>d__14::<>2__current
	RuntimeObject* ___U3CU3E2__current_1;
	// Tutorial Tutorial/<TextAnimation>d__14::<>4__this
	Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* ___U3CU3E4__this_2;
	// System.String Tutorial/<TextAnimation>d__14::str
	String_t* ___str_3;
	// System.String Tutorial/<TextAnimation>d__14::<>7__wrap1
	String_t* ___U3CU3E7__wrap1_4;
	// System.Int32 Tutorial/<TextAnimation>d__14::<>7__wrap2
	int32_t ___U3CU3E7__wrap2_5;
};

// System.Collections.Generic.List`1/Enumerator<Dice>
struct Enumerator_t4FA7DAAD85BE6888B74F6C499212CB3463719F5D 
{
	// System.Collections.Generic.List`1<T> System.Collections.Generic.List`1/Enumerator::_list
	List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* ____list_0;
	// System.Int32 System.Collections.Generic.List`1/Enumerator::_index
	int32_t ____index_1;
	// System.Int32 System.Collections.Generic.List`1/Enumerator::_version
	int32_t ____version_2;
	// T System.Collections.Generic.List`1/Enumerator::_current
	Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* ____current_3;
};

// TMPro.TMP_TextProcessingStack`1<System.Int32>
struct TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C 
{
	// T[] TMPro.TMP_TextProcessingStack`1::itemStack
	Int32U5BU5D_t19C97395396A72ECAF310612F0760F165060314C* ___itemStack_0;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::index
	int32_t ___index_1;
	// T TMPro.TMP_TextProcessingStack`1::m_DefaultItem
	int32_t ___m_DefaultItem_2;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Capacity
	int32_t ___m_Capacity_3;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_RolloverSize
	int32_t ___m_RolloverSize_4;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Count
	int32_t ___m_Count_5;
};

// TMPro.TMP_TextProcessingStack`1<System.Single>
struct TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 
{
	// T[] TMPro.TMP_TextProcessingStack`1::itemStack
	SingleU5BU5D_t89DEFE97BCEDB5857010E79ECE0F52CF6E93B87C* ___itemStack_0;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::index
	int32_t ___index_1;
	// T TMPro.TMP_TextProcessingStack`1::m_DefaultItem
	float ___m_DefaultItem_2;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Capacity
	int32_t ___m_Capacity_3;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_RolloverSize
	int32_t ___m_RolloverSize_4;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Count
	int32_t ___m_Count_5;
};

// TMPro.TMP_TextProcessingStack`1<TMPro.TMP_ColorGradient>
struct TMP_TextProcessingStack_1_tC8FAEB17246D3B171EFD11165A5761AE39B40D0C 
{
	// T[] TMPro.TMP_TextProcessingStack`1::itemStack
	TMP_ColorGradientU5BU5D_t2F65E8C42F268DFF33BB1392D94BCF5B5087308A* ___itemStack_0;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::index
	int32_t ___index_1;
	// T TMPro.TMP_TextProcessingStack`1::m_DefaultItem
	TMP_ColorGradient_t17B51752B4E9499A1FF7D875DCEC1D15A0F4AEBB* ___m_DefaultItem_2;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Capacity
	int32_t ___m_Capacity_3;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_RolloverSize
	int32_t ___m_RolloverSize_4;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Count
	int32_t ___m_Count_5;
};

// System.Boolean
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22 
{
	// System.Boolean System.Boolean::m_value
	bool ___m_value_0;
};

struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22_StaticFields
{
	// System.String System.Boolean::TrueString
	String_t* ___TrueString_5;
	// System.String System.Boolean::FalseString
	String_t* ___FalseString_6;
};

// System.Char
struct Char_t521A6F19B456D956AF452D926C32709DC03D6B17 
{
	// System.Char System.Char::m_value
	Il2CppChar ___m_value_0;
};

struct Char_t521A6F19B456D956AF452D926C32709DC03D6B17_StaticFields
{
	// System.Byte[] System.Char::s_categoryForLatin1
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___s_categoryForLatin1_3;
};

// UnityEngine.Color
struct Color_tD001788D726C3A7F1379BEED0260B9591F440C1F 
{
	// System.Single UnityEngine.Color::r
	float ___r_0;
	// System.Single UnityEngine.Color::g
	float ___g_1;
	// System.Single UnityEngine.Color::b
	float ___b_2;
	// System.Single UnityEngine.Color::a
	float ___a_3;
};

// UnityEngine.Color32
struct Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B 
{
	union
	{
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Int32 UnityEngine.Color32::rgba
			int32_t ___rgba_0;
		};
		#pragma pack(pop, tp)
		struct
		{
			int32_t ___rgba_0_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Byte UnityEngine.Color32::r
			uint8_t ___r_1;
		};
		#pragma pack(pop, tp)
		struct
		{
			uint8_t ___r_1_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___g_2_OffsetPadding[1];
			// System.Byte UnityEngine.Color32::g
			uint8_t ___g_2;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___g_2_OffsetPadding_forAlignmentOnly[1];
			uint8_t ___g_2_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___b_3_OffsetPadding[2];
			// System.Byte UnityEngine.Color32::b
			uint8_t ___b_3;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___b_3_OffsetPadding_forAlignmentOnly[2];
			uint8_t ___b_3_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___a_4_OffsetPadding[3];
			// System.Byte UnityEngine.Color32::a
			uint8_t ___a_4;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___a_4_OffsetPadding_forAlignmentOnly[3];
			uint8_t ___a_4_forAlignmentOnly;
		};
	};
};

// System.Enum
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2  : public ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F
{
};

struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_StaticFields
{
	// System.Char[] System.Enum::enumSeperatorCharArray
	CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB* ___enumSeperatorCharArray_0;
};
// Native definition for P/Invoke marshalling of System.Enum
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.Enum
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_com
{
};

// System.Int32
struct Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C 
{
	// System.Int32 System.Int32::m_value
	int32_t ___m_value_0;
};

// System.IntPtr
struct IntPtr_t 
{
	// System.Void* System.IntPtr::m_value
	void* ___m_value_0;
};

struct IntPtr_t_StaticFields
{
	// System.IntPtr System.IntPtr::Zero
	intptr_t ___Zero_1;
};

// TMPro.MaterialReference
struct MaterialReference_tFD98FFFBBDF168028E637446C6676507186F4D0B 
{
	// System.Int32 TMPro.MaterialReference::index
	int32_t ___index_0;
	// TMPro.TMP_FontAsset TMPro.MaterialReference::fontAsset
	TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160* ___fontAsset_1;
	// TMPro.TMP_SpriteAsset TMPro.MaterialReference::spriteAsset
	TMP_SpriteAsset_t81F779E6F705CE190DC0D1F93A954CB8B1774B39* ___spriteAsset_2;
	// UnityEngine.Material TMPro.MaterialReference::material
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___material_3;
	// System.Boolean TMPro.MaterialReference::isDefaultMaterial
	bool ___isDefaultMaterial_4;
	// System.Boolean TMPro.MaterialReference::isFallbackMaterial
	bool ___isFallbackMaterial_5;
	// UnityEngine.Material TMPro.MaterialReference::fallbackMaterial
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___fallbackMaterial_6;
	// System.Single TMPro.MaterialReference::padding
	float ___padding_7;
	// System.Int32 TMPro.MaterialReference::referenceCount
	int32_t ___referenceCount_8;
};
// Native definition for P/Invoke marshalling of TMPro.MaterialReference
struct MaterialReference_tFD98FFFBBDF168028E637446C6676507186F4D0B_marshaled_pinvoke
{
	int32_t ___index_0;
	TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160* ___fontAsset_1;
	TMP_SpriteAsset_t81F779E6F705CE190DC0D1F93A954CB8B1774B39* ___spriteAsset_2;
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___material_3;
	int32_t ___isDefaultMaterial_4;
	int32_t ___isFallbackMaterial_5;
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___fallbackMaterial_6;
	float ___padding_7;
	int32_t ___referenceCount_8;
};
// Native definition for COM marshalling of TMPro.MaterialReference
struct MaterialReference_tFD98FFFBBDF168028E637446C6676507186F4D0B_marshaled_com
{
	int32_t ___index_0;
	TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160* ___fontAsset_1;
	TMP_SpriteAsset_t81F779E6F705CE190DC0D1F93A954CB8B1774B39* ___spriteAsset_2;
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___material_3;
	int32_t ___isDefaultMaterial_4;
	int32_t ___isFallbackMaterial_5;
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___fallbackMaterial_6;
	float ___padding_7;
	int32_t ___referenceCount_8;
};

// UnityEngine.Matrix4x4
struct Matrix4x4_tDB70CF134A14BA38190C59AA700BCE10E2AED3E6 
{
	// System.Single UnityEngine.Matrix4x4::m00
	float ___m00_0;
	// System.Single UnityEngine.Matrix4x4::m10
	float ___m10_1;
	// System.Single UnityEngine.Matrix4x4::m20
	float ___m20_2;
	// System.Single UnityEngine.Matrix4x4::m30
	float ___m30_3;
	// System.Single UnityEngine.Matrix4x4::m01
	float ___m01_4;
	// System.Single UnityEngine.Matrix4x4::m11
	float ___m11_5;
	// System.Single UnityEngine.Matrix4x4::m21
	float ___m21_6;
	// System.Single UnityEngine.Matrix4x4::m31
	float ___m31_7;
	// System.Single UnityEngine.Matrix4x4::m02
	float ___m02_8;
	// System.Single UnityEngine.Matrix4x4::m12
	float ___m12_9;
	// System.Single UnityEngine.Matrix4x4::m22
	float ___m22_10;
	// System.Single UnityEngine.Matrix4x4::m32
	float ___m32_11;
	// System.Single UnityEngine.Matrix4x4::m03
	float ___m03_12;
	// System.Single UnityEngine.Matrix4x4::m13
	float ___m13_13;
	// System.Single UnityEngine.Matrix4x4::m23
	float ___m23_14;
	// System.Single UnityEngine.Matrix4x4::m33
	float ___m33_15;
};

struct Matrix4x4_tDB70CF134A14BA38190C59AA700BCE10E2AED3E6_StaticFields
{
	// UnityEngine.Matrix4x4 UnityEngine.Matrix4x4::zeroMatrix
	Matrix4x4_tDB70CF134A14BA38190C59AA700BCE10E2AED3E6 ___zeroMatrix_16;
	// UnityEngine.Matrix4x4 UnityEngine.Matrix4x4::identityMatrix
	Matrix4x4_tDB70CF134A14BA38190C59AA700BCE10E2AED3E6 ___identityMatrix_17;
};

// UnityEngine.Rect
struct Rect_tA04E0F8A1830E767F40FB27ECD8D309303571F0D 
{
	// System.Single UnityEngine.Rect::m_XMin
	float ___m_XMin_0;
	// System.Single UnityEngine.Rect::m_YMin
	float ___m_YMin_1;
	// System.Single UnityEngine.Rect::m_Width
	float ___m_Width_2;
	// System.Single UnityEngine.Rect::m_Height
	float ___m_Height_3;
};

// UnityEngine.SceneManagement.Scene
struct Scene_tA1DC762B79745EB5140F054C884855B922318356 
{
	// System.Int32 UnityEngine.SceneManagement.Scene::m_Handle
	int32_t ___m_Handle_0;
};

// System.Single
struct Single_t4530F2FF86FCB0DC29F35385CA1BD21BE294761C 
{
	// System.Single System.Single::m_value
	float ___m_value_0;
};

// TMPro.TMP_FontStyleStack
struct TMP_FontStyleStack_t52885F172FADBC21346C835B5302167BDA8020DC 
{
	// System.Byte TMPro.TMP_FontStyleStack::bold
	uint8_t ___bold_0;
	// System.Byte TMPro.TMP_FontStyleStack::italic
	uint8_t ___italic_1;
	// System.Byte TMPro.TMP_FontStyleStack::underline
	uint8_t ___underline_2;
	// System.Byte TMPro.TMP_FontStyleStack::strikethrough
	uint8_t ___strikethrough_3;
	// System.Byte TMPro.TMP_FontStyleStack::highlight
	uint8_t ___highlight_4;
	// System.Byte TMPro.TMP_FontStyleStack::superscript
	uint8_t ___superscript_5;
	// System.Byte TMPro.TMP_FontStyleStack::subscript
	uint8_t ___subscript_6;
	// System.Byte TMPro.TMP_FontStyleStack::uppercase
	uint8_t ___uppercase_7;
	// System.Byte TMPro.TMP_FontStyleStack::lowercase
	uint8_t ___lowercase_8;
	// System.Byte TMPro.TMP_FontStyleStack::smallcaps
	uint8_t ___smallcaps_9;
};

// TMPro.TMP_Offset
struct TMP_Offset_t2262BE4E87D9662487777FF8FFE1B17B0E4438C6 
{
	// System.Single TMPro.TMP_Offset::m_Left
	float ___m_Left_0;
	// System.Single TMPro.TMP_Offset::m_Right
	float ___m_Right_1;
	// System.Single TMPro.TMP_Offset::m_Top
	float ___m_Top_2;
	// System.Single TMPro.TMP_Offset::m_Bottom
	float ___m_Bottom_3;
};

struct TMP_Offset_t2262BE4E87D9662487777FF8FFE1B17B0E4438C6_StaticFields
{
	// TMPro.TMP_Offset TMPro.TMP_Offset::k_ZeroOffset
	TMP_Offset_t2262BE4E87D9662487777FF8FFE1B17B0E4438C6 ___k_ZeroOffset_4;
};

// System.UInt32
struct UInt32_t1833D51FFA667B18A5AA4B8D34DE284F8495D29B 
{
	// System.UInt32 System.UInt32::m_value
	uint32_t ___m_value_0;
};

// UnityEngine.Vector2
struct Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 
{
	// System.Single UnityEngine.Vector2::x
	float ___x_0;
	// System.Single UnityEngine.Vector2::y
	float ___y_1;
};

struct Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7_StaticFields
{
	// UnityEngine.Vector2 UnityEngine.Vector2::zeroVector
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___zeroVector_2;
	// UnityEngine.Vector2 UnityEngine.Vector2::oneVector
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___oneVector_3;
	// UnityEngine.Vector2 UnityEngine.Vector2::upVector
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___upVector_4;
	// UnityEngine.Vector2 UnityEngine.Vector2::downVector
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___downVector_5;
	// UnityEngine.Vector2 UnityEngine.Vector2::leftVector
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___leftVector_6;
	// UnityEngine.Vector2 UnityEngine.Vector2::rightVector
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___rightVector_7;
	// UnityEngine.Vector2 UnityEngine.Vector2::positiveInfinityVector
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___positiveInfinityVector_8;
	// UnityEngine.Vector2 UnityEngine.Vector2::negativeInfinityVector
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___negativeInfinityVector_9;
};

// UnityEngine.Vector3
struct Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 
{
	// System.Single UnityEngine.Vector3::x
	float ___x_2;
	// System.Single UnityEngine.Vector3::y
	float ___y_3;
	// System.Single UnityEngine.Vector3::z
	float ___z_4;
};

struct Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_StaticFields
{
	// UnityEngine.Vector3 UnityEngine.Vector3::zeroVector
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___zeroVector_5;
	// UnityEngine.Vector3 UnityEngine.Vector3::oneVector
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___oneVector_6;
	// UnityEngine.Vector3 UnityEngine.Vector3::upVector
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___upVector_7;
	// UnityEngine.Vector3 UnityEngine.Vector3::downVector
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___downVector_8;
	// UnityEngine.Vector3 UnityEngine.Vector3::leftVector
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___leftVector_9;
	// UnityEngine.Vector3 UnityEngine.Vector3::rightVector
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___rightVector_10;
	// UnityEngine.Vector3 UnityEngine.Vector3::forwardVector
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___forwardVector_11;
	// UnityEngine.Vector3 UnityEngine.Vector3::backVector
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___backVector_12;
	// UnityEngine.Vector3 UnityEngine.Vector3::positiveInfinityVector
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___positiveInfinityVector_13;
	// UnityEngine.Vector3 UnityEngine.Vector3::negativeInfinityVector
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___negativeInfinityVector_14;
};

// UnityEngine.Vector4
struct Vector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3 
{
	// System.Single UnityEngine.Vector4::x
	float ___x_1;
	// System.Single UnityEngine.Vector4::y
	float ___y_2;
	// System.Single UnityEngine.Vector4::z
	float ___z_3;
	// System.Single UnityEngine.Vector4::w
	float ___w_4;
};

struct Vector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3_StaticFields
{
	// UnityEngine.Vector4 UnityEngine.Vector4::zeroVector
	Vector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3 ___zeroVector_5;
	// UnityEngine.Vector4 UnityEngine.Vector4::oneVector
	Vector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3 ___oneVector_6;
	// UnityEngine.Vector4 UnityEngine.Vector4::positiveInfinityVector
	Vector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3 ___positiveInfinityVector_7;
	// UnityEngine.Vector4 UnityEngine.Vector4::negativeInfinityVector
	Vector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3 ___negativeInfinityVector_8;
};

// System.Void
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915 
{
	union
	{
		struct
		{
		};
		uint8_t Void_t4861ACF8F4594C3437BB48B6E56783494B843915__padding[1];
	};
};

// UnityEngine.WaitForSeconds
struct WaitForSeconds_tF179DF251655B8DF044952E70A60DF4B358A3DD3  : public YieldInstruction_tFCE35FD0907950EFEE9BC2890AC664E41C53728D
{
	// System.Single UnityEngine.WaitForSeconds::m_Seconds
	float ___m_Seconds_0;
};
// Native definition for P/Invoke marshalling of UnityEngine.WaitForSeconds
struct WaitForSeconds_tF179DF251655B8DF044952E70A60DF4B358A3DD3_marshaled_pinvoke : public YieldInstruction_tFCE35FD0907950EFEE9BC2890AC664E41C53728D_marshaled_pinvoke
{
	float ___m_Seconds_0;
};
// Native definition for COM marshalling of UnityEngine.WaitForSeconds
struct WaitForSeconds_tF179DF251655B8DF044952E70A60DF4B358A3DD3_marshaled_com : public YieldInstruction_tFCE35FD0907950EFEE9BC2890AC664E41C53728D_marshaled_com
{
	float ___m_Seconds_0;
};

// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=128
struct __StaticArrayInitTypeSizeU3D128_tF4DC60A802E7EAF26084A16B33B2CDCC817796AB 
{
	union
	{
		struct
		{
			union
			{
			};
		};
		uint8_t __StaticArrayInitTypeSizeU3D128_tF4DC60A802E7EAF26084A16B33B2CDCC817796AB__padding[128];
	};
};

// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=16
struct __StaticArrayInitTypeSizeU3D16_tFB2D94E174C3DFBC336BBEE6AD92E07462831A23 
{
	union
	{
		struct
		{
			union
			{
			};
		};
		uint8_t __StaticArrayInitTypeSizeU3D16_tFB2D94E174C3DFBC336BBEE6AD92E07462831A23__padding[16];
	};
};

// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=24
struct __StaticArrayInitTypeSizeU3D24_t3464DA68B6CCAB9A0A43F94B3DB9AA7E7FDDB19A 
{
	union
	{
		struct
		{
			union
			{
			};
		};
		uint8_t __StaticArrayInitTypeSizeU3D24_t3464DA68B6CCAB9A0A43F94B3DB9AA7E7FDDB19A__padding[24];
	};
};

// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=40
struct __StaticArrayInitTypeSizeU3D40_t68A41E1D2BAA1C55857C26F7E0C26D1CFDB100B3 
{
	union
	{
		struct
		{
			union
			{
			};
		};
		uint8_t __StaticArrayInitTypeSizeU3D40_t68A41E1D2BAA1C55857C26F7E0C26D1CFDB100B3__padding[40];
	};
};

// TMPro.TMP_Text/SpecialCharacter
struct SpecialCharacter_t6C1DBE8C490706D1620899BAB7F0B8091AD26777 
{
	// TMPro.TMP_Character TMPro.TMP_Text/SpecialCharacter::character
	TMP_Character_t7D37A55EF1A9FF6D0BFE6D50E86A00F80E7FAF35* ___character_0;
	// TMPro.TMP_FontAsset TMPro.TMP_Text/SpecialCharacter::fontAsset
	TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160* ___fontAsset_1;
	// UnityEngine.Material TMPro.TMP_Text/SpecialCharacter::material
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___material_2;
	// System.Int32 TMPro.TMP_Text/SpecialCharacter::materialIndex
	int32_t ___materialIndex_3;
};
// Native definition for P/Invoke marshalling of TMPro.TMP_Text/SpecialCharacter
struct SpecialCharacter_t6C1DBE8C490706D1620899BAB7F0B8091AD26777_marshaled_pinvoke
{
	TMP_Character_t7D37A55EF1A9FF6D0BFE6D50E86A00F80E7FAF35* ___character_0;
	TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160* ___fontAsset_1;
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___material_2;
	int32_t ___materialIndex_3;
};
// Native definition for COM marshalling of TMPro.TMP_Text/SpecialCharacter
struct SpecialCharacter_t6C1DBE8C490706D1620899BAB7F0B8091AD26777_marshaled_com
{
	TMP_Character_t7D37A55EF1A9FF6D0BFE6D50E86A00F80E7FAF35* ___character_0;
	TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160* ___fontAsset_1;
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___material_2;
	int32_t ___materialIndex_3;
};

// TMPro.TMP_Text/TextBackingContainer
struct TextBackingContainer_t33D1CE628E7B26C45EDAC1D87BEF2DD22A5C6361 
{
	// System.UInt32[] TMPro.TMP_Text/TextBackingContainer::m_Array
	UInt32U5BU5D_t02FBD658AD156A17574ECE6106CF1FBFCC9807FA* ___m_Array_0;
	// System.Int32 TMPro.TMP_Text/TextBackingContainer::m_Count
	int32_t ___m_Count_1;
};
// Native definition for P/Invoke marshalling of TMPro.TMP_Text/TextBackingContainer
struct TextBackingContainer_t33D1CE628E7B26C45EDAC1D87BEF2DD22A5C6361_marshaled_pinvoke
{
	Il2CppSafeArray/*NONE*/* ___m_Array_0;
	int32_t ___m_Count_1;
};
// Native definition for COM marshalling of TMPro.TMP_Text/TextBackingContainer
struct TextBackingContainer_t33D1CE628E7B26C45EDAC1D87BEF2DD22A5C6361_marshaled_com
{
	Il2CppSafeArray/*NONE*/* ___m_Array_0;
	int32_t ___m_Count_1;
};

// System.Collections.Generic.List`1/Enumerator<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>
typedef Il2CppFullySharedGenericStruct Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF;

// TMPro.TMP_TextProcessingStack`1<UnityEngine.Color32>
struct TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 
{
	// T[] TMPro.TMP_TextProcessingStack`1::itemStack
	Color32U5BU5D_t38116C3E91765C4C5726CE12C77FAD7F9F737259* ___itemStack_0;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::index
	int32_t ___index_1;
	// T TMPro.TMP_TextProcessingStack`1::m_DefaultItem
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___m_DefaultItem_2;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Capacity
	int32_t ___m_Capacity_3;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_RolloverSize
	int32_t ___m_RolloverSize_4;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Count
	int32_t ___m_Count_5;
};

// TMPro.TMP_TextProcessingStack`1<TMPro.MaterialReference>
struct TMP_TextProcessingStack_1_tB03E08F69415B281A5A81138F09E49EE58402DF9 
{
	// T[] TMPro.TMP_TextProcessingStack`1::itemStack
	MaterialReferenceU5BU5D_t7491D335AB3E3E13CE9C0F5E931F396F6A02E1F2* ___itemStack_0;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::index
	int32_t ___index_1;
	// T TMPro.TMP_TextProcessingStack`1::m_DefaultItem
	MaterialReference_tFD98FFFBBDF168028E637446C6676507186F4D0B ___m_DefaultItem_2;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Capacity
	int32_t ___m_Capacity_3;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_RolloverSize
	int32_t ___m_RolloverSize_4;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Count
	int32_t ___m_Count_5;
};

// <PrivateImplementationDetails>
struct U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA  : public RuntimeObject
{
};

struct U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA_StaticFields
{
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=16 <PrivateImplementationDetails>::0203C37BFC80A19C11627472CBDE44F967DAFAE99ACAB0F42E73E35A398997C4
	__StaticArrayInitTypeSizeU3D16_tFB2D94E174C3DFBC336BBEE6AD92E07462831A23 ___0203C37BFC80A19C11627472CBDE44F967DAFAE99ACAB0F42E73E35A398997C4_0;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=40 <PrivateImplementationDetails>::08446D7774EF53C3E5B1C9EE943CEF52E3EB9F5FA8FA86C4513E7B453CE1F134
	__StaticArrayInitTypeSizeU3D40_t68A41E1D2BAA1C55857C26F7E0C26D1CFDB100B3 ___08446D7774EF53C3E5B1C9EE943CEF52E3EB9F5FA8FA86C4513E7B453CE1F134_1;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=24 <PrivateImplementationDetails>::1FB97A1DFE78D75E355D1593EB37EAF7F766C65A0C522DFC2870CFCB794805A5
	__StaticArrayInitTypeSizeU3D24_t3464DA68B6CCAB9A0A43F94B3DB9AA7E7FDDB19A ___1FB97A1DFE78D75E355D1593EB37EAF7F766C65A0C522DFC2870CFCB794805A5_2;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=40 <PrivateImplementationDetails>::29BBCDFAED1C6E824F7A28ECF1DD9D5816BDAFCBBED36B6AECFB4DF5ABA2229B
	__StaticArrayInitTypeSizeU3D40_t68A41E1D2BAA1C55857C26F7E0C26D1CFDB100B3 ___29BBCDFAED1C6E824F7A28ECF1DD9D5816BDAFCBBED36B6AECFB4DF5ABA2229B_3;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=128 <PrivateImplementationDetails>::2DA01C084AD002EA53506AAD76E99F3E3F69AB5B07C1435FDED3EDC0EA0F777D
	__StaticArrayInitTypeSizeU3D128_tF4DC60A802E7EAF26084A16B33B2CDCC817796AB ___2DA01C084AD002EA53506AAD76E99F3E3F69AB5B07C1435FDED3EDC0EA0F777D_4;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=40 <PrivateImplementationDetails>::65CE55CCB311221287185F5220552D03D553A95836FEF15299AE5FBD60FF3600
	__StaticArrayInitTypeSizeU3D40_t68A41E1D2BAA1C55857C26F7E0C26D1CFDB100B3 ___65CE55CCB311221287185F5220552D03D553A95836FEF15299AE5FBD60FF3600_5;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=40 <PrivateImplementationDetails>::6A657FA80B47447A8759CC522C73BE428D20693672E01CBF0F00A041739BB978
	__StaticArrayInitTypeSizeU3D40_t68A41E1D2BAA1C55857C26F7E0C26D1CFDB100B3 ___6A657FA80B47447A8759CC522C73BE428D20693672E01CBF0F00A041739BB978_6;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=40 <PrivateImplementationDetails>::710DD8DD9AB6F4508DC5A0EF964CAE593299CBAF3F3C5415D286D649A5FC42FB
	__StaticArrayInitTypeSizeU3D40_t68A41E1D2BAA1C55857C26F7E0C26D1CFDB100B3 ___710DD8DD9AB6F4508DC5A0EF964CAE593299CBAF3F3C5415D286D649A5FC42FB_7;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=16 <PrivateImplementationDetails>::8545B2C8C636C949C19BAA34BE965162D28BC2732D255E61874C91DDEE2D5FB7
	__StaticArrayInitTypeSizeU3D16_tFB2D94E174C3DFBC336BBEE6AD92E07462831A23 ___8545B2C8C636C949C19BAA34BE965162D28BC2732D255E61874C91DDEE2D5FB7_8;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=40 <PrivateImplementationDetails>::8D0BE4BC9355691BF6EAC3BE7B11F77C0994ED8080D01888FF1C98EB886B3B6B
	__StaticArrayInitTypeSizeU3D40_t68A41E1D2BAA1C55857C26F7E0C26D1CFDB100B3 ___8D0BE4BC9355691BF6EAC3BE7B11F77C0994ED8080D01888FF1C98EB886B3B6B_9;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=40 <PrivateImplementationDetails>::A1E610F78FBB531301B32D29E197F12C14ED0C4D2C2FF8F28768B754DD40E396
	__StaticArrayInitTypeSizeU3D40_t68A41E1D2BAA1C55857C26F7E0C26D1CFDB100B3 ___A1E610F78FBB531301B32D29E197F12C14ED0C4D2C2FF8F28768B754DD40E396_10;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=16 <PrivateImplementationDetails>::B6A26C96D971F6678579C0756EB094C37FEE88B4A01DD8DCA45E963253752973
	__StaticArrayInitTypeSizeU3D16_tFB2D94E174C3DFBC336BBEE6AD92E07462831A23 ___B6A26C96D971F6678579C0756EB094C37FEE88B4A01DD8DCA45E963253752973_11;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=40 <PrivateImplementationDetails>::BEAC83D04947E4A115D4449D8B070C2A526E394648E16ED47456E6BE26274272
	__StaticArrayInitTypeSizeU3D40_t68A41E1D2BAA1C55857C26F7E0C26D1CFDB100B3 ___BEAC83D04947E4A115D4449D8B070C2A526E394648E16ED47456E6BE26274272_12;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=16 <PrivateImplementationDetails>::BFD997E622FB1FAE0E91C9EA6E45683D5AB360447B8C25F13DAC6BF7B1182DCE
	__StaticArrayInitTypeSizeU3D16_tFB2D94E174C3DFBC336BBEE6AD92E07462831A23 ___BFD997E622FB1FAE0E91C9EA6E45683D5AB360447B8C25F13DAC6BF7B1182DCE_13;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=16 <PrivateImplementationDetails>::C3A6B1F08B0B05AC05390D6C257551FFD0CDCF40496F232B52DF2498F915469E
	__StaticArrayInitTypeSizeU3D16_tFB2D94E174C3DFBC336BBEE6AD92E07462831A23 ___C3A6B1F08B0B05AC05390D6C257551FFD0CDCF40496F232B52DF2498F915469E_14;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=16 <PrivateImplementationDetails>::F6BB1294DA2F78CD935B01C7656280DF5EAA0439E9D97BC03775825A41A508E4
	__StaticArrayInitTypeSizeU3D16_tFB2D94E174C3DFBC336BBEE6AD92E07462831A23 ___F6BB1294DA2F78CD935B01C7656280DF5EAA0439E9D97BC03775825A41A508E4_15;
	// <PrivateImplementationDetails>/__StaticArrayInitTypeSize=40 <PrivateImplementationDetails>::FEBF3C1689897117B5C841445B77822C2CC292E120BAE237E730638166232B0E
	__StaticArrayInitTypeSizeU3D40_t68A41E1D2BAA1C55857C26F7E0C26D1CFDB100B3 ___FEBF3C1689897117B5C841445B77822C2CC292E120BAE237E730638166232B0E_16;
};

// TMPro.ColorMode
struct ColorMode_tA7A815AAB9F175EFBA0AE0814E55728432A880BF 
{
	// System.Int32 TMPro.ColorMode::value__
	int32_t ___value___2;
};

// UnityEngine.Coroutine
struct Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B  : public YieldInstruction_tFCE35FD0907950EFEE9BC2890AC664E41C53728D
{
	// System.IntPtr UnityEngine.Coroutine::m_Ptr
	intptr_t ___m_Ptr_0;
};
// Native definition for P/Invoke marshalling of UnityEngine.Coroutine
struct Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B_marshaled_pinvoke : public YieldInstruction_tFCE35FD0907950EFEE9BC2890AC664E41C53728D_marshaled_pinvoke
{
	intptr_t ___m_Ptr_0;
};
// Native definition for COM marshalling of UnityEngine.Coroutine
struct Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B_marshaled_com : public YieldInstruction_tFCE35FD0907950EFEE9BC2890AC664E41C53728D_marshaled_com
{
	intptr_t ___m_Ptr_0;
};

// System.Delegate
struct Delegate_t  : public RuntimeObject
{
	// System.IntPtr System.Delegate::method_ptr
	Il2CppMethodPointer ___method_ptr_0;
	// System.IntPtr System.Delegate::invoke_impl
	intptr_t ___invoke_impl_1;
	// System.Object System.Delegate::m_target
	RuntimeObject* ___m_target_2;
	// System.IntPtr System.Delegate::method
	intptr_t ___method_3;
	// System.IntPtr System.Delegate::delegate_trampoline
	intptr_t ___delegate_trampoline_4;
	// System.IntPtr System.Delegate::extra_arg
	intptr_t ___extra_arg_5;
	// System.IntPtr System.Delegate::method_code
	intptr_t ___method_code_6;
	// System.IntPtr System.Delegate::interp_method
	intptr_t ___interp_method_7;
	// System.IntPtr System.Delegate::interp_invoke_impl
	intptr_t ___interp_invoke_impl_8;
	// System.Reflection.MethodInfo System.Delegate::method_info
	MethodInfo_t* ___method_info_9;
	// System.Reflection.MethodInfo System.Delegate::original_method_info
	MethodInfo_t* ___original_method_info_10;
	// System.DelegateData System.Delegate::data
	DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E* ___data_11;
	// System.Boolean System.Delegate::method_is_virtual
	bool ___method_is_virtual_12;
};
// Native definition for P/Invoke marshalling of System.Delegate
struct Delegate_t_marshaled_pinvoke
{
	intptr_t ___method_ptr_0;
	intptr_t ___invoke_impl_1;
	Il2CppIUnknown* ___m_target_2;
	intptr_t ___method_3;
	intptr_t ___delegate_trampoline_4;
	intptr_t ___extra_arg_5;
	intptr_t ___method_code_6;
	intptr_t ___interp_method_7;
	intptr_t ___interp_invoke_impl_8;
	MethodInfo_t* ___method_info_9;
	MethodInfo_t* ___original_method_info_10;
	DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E* ___data_11;
	int32_t ___method_is_virtual_12;
};
// Native definition for COM marshalling of System.Delegate
struct Delegate_t_marshaled_com
{
	intptr_t ___method_ptr_0;
	intptr_t ___invoke_impl_1;
	Il2CppIUnknown* ___m_target_2;
	intptr_t ___method_3;
	intptr_t ___delegate_trampoline_4;
	intptr_t ___extra_arg_5;
	intptr_t ___method_code_6;
	intptr_t ___interp_method_7;
	intptr_t ___interp_invoke_impl_8;
	MethodInfo_t* ___method_info_9;
	MethodInfo_t* ___original_method_info_10;
	DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E* ___data_11;
	int32_t ___method_is_virtual_12;
};

// System.Exception
struct Exception_t  : public RuntimeObject
{
	// System.String System.Exception::_className
	String_t* ____className_1;
	// System.String System.Exception::_message
	String_t* ____message_2;
	// System.Collections.IDictionary System.Exception::_data
	RuntimeObject* ____data_3;
	// System.Exception System.Exception::_innerException
	Exception_t* ____innerException_4;
	// System.String System.Exception::_helpURL
	String_t* ____helpURL_5;
	// System.Object System.Exception::_stackTrace
	RuntimeObject* ____stackTrace_6;
	// System.String System.Exception::_stackTraceString
	String_t* ____stackTraceString_7;
	// System.String System.Exception::_remoteStackTraceString
	String_t* ____remoteStackTraceString_8;
	// System.Int32 System.Exception::_remoteStackIndex
	int32_t ____remoteStackIndex_9;
	// System.Object System.Exception::_dynamicMethods
	RuntimeObject* ____dynamicMethods_10;
	// System.Int32 System.Exception::_HResult
	int32_t ____HResult_11;
	// System.String System.Exception::_source
	String_t* ____source_12;
	// System.Runtime.Serialization.SafeSerializationManager System.Exception::_safeSerializationManager
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager_13;
	// System.Diagnostics.StackTrace[] System.Exception::captured_traces
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces_14;
	// System.IntPtr[] System.Exception::native_trace_ips
	IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___native_trace_ips_15;
	// System.Int32 System.Exception::caught_in_unmanaged
	int32_t ___caught_in_unmanaged_16;
};

struct Exception_t_StaticFields
{
	// System.Object System.Exception::s_EDILock
	RuntimeObject* ___s_EDILock_0;
};
// Native definition for P/Invoke marshalling of System.Exception
struct Exception_t_marshaled_pinvoke
{
	char* ____className_1;
	char* ____message_2;
	RuntimeObject* ____data_3;
	Exception_t_marshaled_pinvoke* ____innerException_4;
	char* ____helpURL_5;
	Il2CppIUnknown* ____stackTrace_6;
	char* ____stackTraceString_7;
	char* ____remoteStackTraceString_8;
	int32_t ____remoteStackIndex_9;
	Il2CppIUnknown* ____dynamicMethods_10;
	int32_t ____HResult_11;
	char* ____source_12;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager_13;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces_14;
	Il2CppSafeArray/*NONE*/* ___native_trace_ips_15;
	int32_t ___caught_in_unmanaged_16;
};
// Native definition for COM marshalling of System.Exception
struct Exception_t_marshaled_com
{
	Il2CppChar* ____className_1;
	Il2CppChar* ____message_2;
	RuntimeObject* ____data_3;
	Exception_t_marshaled_com* ____innerException_4;
	Il2CppChar* ____helpURL_5;
	Il2CppIUnknown* ____stackTrace_6;
	Il2CppChar* ____stackTraceString_7;
	Il2CppChar* ____remoteStackTraceString_8;
	int32_t ____remoteStackIndex_9;
	Il2CppIUnknown* ____dynamicMethods_10;
	int32_t ____HResult_11;
	Il2CppChar* ____source_12;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager_13;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces_14;
	Il2CppSafeArray/*NONE*/* ___native_trace_ips_15;
	int32_t ___caught_in_unmanaged_16;
};

// TMPro.Extents
struct Extents_tA2D2F95811D0A18CB7AC3570D2D8F8CD3AF4C4A8 
{
	// UnityEngine.Vector2 TMPro.Extents::min
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___min_2;
	// UnityEngine.Vector2 TMPro.Extents::max
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___max_3;
};

struct Extents_tA2D2F95811D0A18CB7AC3570D2D8F8CD3AF4C4A8_StaticFields
{
	// TMPro.Extents TMPro.Extents::zero
	Extents_tA2D2F95811D0A18CB7AC3570D2D8F8CD3AF4C4A8 ___zero_0;
	// TMPro.Extents TMPro.Extents::uninitialized
	Extents_tA2D2F95811D0A18CB7AC3570D2D8F8CD3AF4C4A8 ___uninitialized_1;
};

// TMPro.FontStyles
struct FontStyles_t9E611EE6BBE6E192A73EAFF7872596517C527FF5 
{
	// System.Int32 TMPro.FontStyles::value__
	int32_t ___value___2;
};

// TMPro.FontWeight
struct FontWeight_tA2585C0A73B70D31CE71E7843149098A5E16BC80 
{
	// System.Int32 TMPro.FontWeight::value__
	int32_t ___value___2;
};

// TMPro.HighlightState
struct HighlightState_tE4F50287E5E2E91D42AB77DEA281D88D3AD6A28B 
{
	// UnityEngine.Color32 TMPro.HighlightState::color
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___color_0;
	// TMPro.TMP_Offset TMPro.HighlightState::padding
	TMP_Offset_t2262BE4E87D9662487777FF8FFE1B17B0E4438C6 ___padding_1;
};

// TMPro.HorizontalAlignmentOptions
struct HorizontalAlignmentOptions_tCC21260E9FBEC656BA7783643ED5F44AFF7955A1 
{
	// System.Int32 TMPro.HorizontalAlignmentOptions::value__
	int32_t ___value___2;
};

// UnityEngine.SceneManagement.LoadSceneMode
struct LoadSceneMode_t3E17ADA25A3C4F14ECF6026741219437DA054963 
{
	// System.Int32 UnityEngine.SceneManagement.LoadSceneMode::value__
	int32_t ___value___2;
};

// UnityEngine.Object
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C  : public RuntimeObject
{
	// System.IntPtr UnityEngine.Object::m_CachedPtr
	intptr_t ___m_CachedPtr_0;
};

struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_StaticFields
{
	// System.Int32 UnityEngine.Object::OffsetOfInstanceIDInCPlusPlusObject
	int32_t ___OffsetOfInstanceIDInCPlusPlusObject_1;
};
// Native definition for P/Invoke marshalling of UnityEngine.Object
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_marshaled_pinvoke
{
	intptr_t ___m_CachedPtr_0;
};
// Native definition for COM marshalling of UnityEngine.Object
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_marshaled_com
{
	intptr_t ___m_CachedPtr_0;
};

// Unity.Profiling.ProfilerMarker
struct ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD 
{
	// System.IntPtr Unity.Profiling.ProfilerMarker::m_Ptr
	intptr_t ___m_Ptr_0;
};

// UnityEngine.RenderMode
struct RenderMode_tB63553E26C26A0B62C47B995F86AC41768494633 
{
	// System.Int32 UnityEngine.RenderMode::value__
	int32_t ___value___2;
};

// TMPro.TMP_TextElementType
struct TMP_TextElementType_t51EE6662436732F22C6B599F5757B7F35F706342 
{
	// System.Int32 TMPro.TMP_TextElementType::value__
	int32_t ___value___2;
};

// TMPro.TextAlignmentOptions
struct TextAlignmentOptions_tF3FA9020F7E2AF1A48660044540254009A22EF01 
{
	// System.Int32 TMPro.TextAlignmentOptions::value__
	int32_t ___value___2;
};

// TMPro.TextOverflowModes
struct TextOverflowModes_t7DCCD00C16E3223CE50CDDCC53F785C0405BE203 
{
	// System.Int32 TMPro.TextOverflowModes::value__
	int32_t ___value___2;
};

// TMPro.TextRenderFlags
struct TextRenderFlags_tE023FF398ECFE57A1DBC6FD2A1AF4AE9620F6E1C 
{
	// System.Int32 TMPro.TextRenderFlags::value__
	int32_t ___value___2;
};

// TMPro.TextureMappingOptions
struct TextureMappingOptions_t0E1A47C529DEB45A875486256E7026E97C940DAE 
{
	// System.Int32 TMPro.TextureMappingOptions::value__
	int32_t ___value___2;
};

// TMPro.VertexGradient
struct VertexGradient_t2C057B53C0EA6E987C2B7BAB0305E686DA1C9A8F 
{
	// UnityEngine.Color TMPro.VertexGradient::topLeft
	Color_tD001788D726C3A7F1379BEED0260B9591F440C1F ___topLeft_0;
	// UnityEngine.Color TMPro.VertexGradient::topRight
	Color_tD001788D726C3A7F1379BEED0260B9591F440C1F ___topRight_1;
	// UnityEngine.Color TMPro.VertexGradient::bottomLeft
	Color_tD001788D726C3A7F1379BEED0260B9591F440C1F ___bottomLeft_2;
	// UnityEngine.Color TMPro.VertexGradient::bottomRight
	Color_tD001788D726C3A7F1379BEED0260B9591F440C1F ___bottomRight_3;
};

// TMPro.VertexSortingOrder
struct VertexSortingOrder_t95B7AEDBDCAACC3459B6476E5CCC594A6422FFA8 
{
	// System.Int32 TMPro.VertexSortingOrder::value__
	int32_t ___value___2;
};

// TMPro.VerticalAlignmentOptions
struct VerticalAlignmentOptions_tCEF70AF60282B71AEEE14D51253CE6A61E72D855 
{
	// System.Int32 TMPro.VerticalAlignmentOptions::value__
	int32_t ___value___2;
};

// UnityEngine.UI.Image/FillMethod
struct FillMethod_t36837ED12068DF1582CC20489D571B0BCAA7AD19 
{
	// System.Int32 UnityEngine.UI.Image/FillMethod::value__
	int32_t ___value___2;
};

// UnityEngine.UI.Image/Type
struct Type_t81D6F138C2FC745112D5247CD91BD483EDFFC041 
{
	// System.Int32 UnityEngine.UI.Image/Type::value__
	int32_t ___value___2;
};

// TMPro.TMP_Text/TextInputSources
struct TextInputSources_t41387D6C9CB16E60390F47A15AEB8185BE966D26 
{
	// System.Int32 TMPro.TMP_Text/TextInputSources::value__
	int32_t ___value___2;
};

// TMPro.TMP_TextProcessingStack`1<TMPro.FontWeight>
struct TMP_TextProcessingStack_1_tA5C8CED87DD9E73F6359E23B334FFB5B6F813FD4 
{
	// T[] TMPro.TMP_TextProcessingStack`1::itemStack
	FontWeightU5BU5D_t2A406B5BAB0DD0F06E7F1773DB062E4AF98067BA* ___itemStack_0;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::index
	int32_t ___index_1;
	// T TMPro.TMP_TextProcessingStack`1::m_DefaultItem
	int32_t ___m_DefaultItem_2;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Capacity
	int32_t ___m_Capacity_3;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_RolloverSize
	int32_t ___m_RolloverSize_4;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Count
	int32_t ___m_Count_5;
};

// TMPro.TMP_TextProcessingStack`1<TMPro.HighlightState>
struct TMP_TextProcessingStack_1_t57AECDCC936A7FF1D6CF66CA11560B28A675648D 
{
	// T[] TMPro.TMP_TextProcessingStack`1::itemStack
	HighlightStateU5BU5D_tA878A0AF1F4F52882ACD29515AADC277EE135622* ___itemStack_0;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::index
	int32_t ___index_1;
	// T TMPro.TMP_TextProcessingStack`1::m_DefaultItem
	HighlightState_tE4F50287E5E2E91D42AB77DEA281D88D3AD6A28B ___m_DefaultItem_2;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Capacity
	int32_t ___m_Capacity_3;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_RolloverSize
	int32_t ___m_RolloverSize_4;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Count
	int32_t ___m_Count_5;
};

// TMPro.TMP_TextProcessingStack`1<TMPro.HorizontalAlignmentOptions>
struct TMP_TextProcessingStack_1_t243EA1B5D7FD2295D6533B953F0BBE8F52EFB8A0 
{
	// T[] TMPro.TMP_TextProcessingStack`1::itemStack
	HorizontalAlignmentOptionsU5BU5D_t4D185662282BFB910D8B9A8199E91578E9422658* ___itemStack_0;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::index
	int32_t ___index_1;
	// T TMPro.TMP_TextProcessingStack`1::m_DefaultItem
	int32_t ___m_DefaultItem_2;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Capacity
	int32_t ___m_Capacity_3;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_RolloverSize
	int32_t ___m_RolloverSize_4;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Count
	int32_t ___m_Count_5;
};

// UnityEngine.Component
struct Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3  : public Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C
{
};

// UnityEngine.GameObject
struct GameObject_t76FEDD663AB33C991A9C9A23129337651094216F  : public Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C
{
};

// System.MulticastDelegate
struct MulticastDelegate_t  : public Delegate_t
{
	// System.Delegate[] System.MulticastDelegate::delegates
	DelegateU5BU5D_tC5AB7E8F745616680F337909D3A8E6C722CDF771* ___delegates_13;
};
// Native definition for P/Invoke marshalling of System.MulticastDelegate
struct MulticastDelegate_t_marshaled_pinvoke : public Delegate_t_marshaled_pinvoke
{
	Delegate_t_marshaled_pinvoke** ___delegates_13;
};
// Native definition for COM marshalling of System.MulticastDelegate
struct MulticastDelegate_t_marshaled_com : public Delegate_t_marshaled_com
{
	Delegate_t_marshaled_com** ___delegates_13;
};

// UnityEngine.Sprite
struct Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99  : public Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C
{
};

// System.SystemException
struct SystemException_tCC48D868298F4C0705279823E34B00F4FBDB7295  : public Exception_t
{
};

// TMPro.TMP_LineInfo
struct TMP_LineInfo_tB75C1965B58DB7B3A046C8CA55AD6AB92B6B17B3 
{
	// System.Int32 TMPro.TMP_LineInfo::controlCharacterCount
	int32_t ___controlCharacterCount_0;
	// System.Int32 TMPro.TMP_LineInfo::characterCount
	int32_t ___characterCount_1;
	// System.Int32 TMPro.TMP_LineInfo::visibleCharacterCount
	int32_t ___visibleCharacterCount_2;
	// System.Int32 TMPro.TMP_LineInfo::spaceCount
	int32_t ___spaceCount_3;
	// System.Int32 TMPro.TMP_LineInfo::wordCount
	int32_t ___wordCount_4;
	// System.Int32 TMPro.TMP_LineInfo::firstCharacterIndex
	int32_t ___firstCharacterIndex_5;
	// System.Int32 TMPro.TMP_LineInfo::firstVisibleCharacterIndex
	int32_t ___firstVisibleCharacterIndex_6;
	// System.Int32 TMPro.TMP_LineInfo::lastCharacterIndex
	int32_t ___lastCharacterIndex_7;
	// System.Int32 TMPro.TMP_LineInfo::lastVisibleCharacterIndex
	int32_t ___lastVisibleCharacterIndex_8;
	// System.Single TMPro.TMP_LineInfo::length
	float ___length_9;
	// System.Single TMPro.TMP_LineInfo::lineHeight
	float ___lineHeight_10;
	// System.Single TMPro.TMP_LineInfo::ascender
	float ___ascender_11;
	// System.Single TMPro.TMP_LineInfo::baseline
	float ___baseline_12;
	// System.Single TMPro.TMP_LineInfo::descender
	float ___descender_13;
	// System.Single TMPro.TMP_LineInfo::maxAdvance
	float ___maxAdvance_14;
	// System.Single TMPro.TMP_LineInfo::width
	float ___width_15;
	// System.Single TMPro.TMP_LineInfo::marginLeft
	float ___marginLeft_16;
	// System.Single TMPro.TMP_LineInfo::marginRight
	float ___marginRight_17;
	// TMPro.HorizontalAlignmentOptions TMPro.TMP_LineInfo::alignment
	int32_t ___alignment_18;
	// TMPro.Extents TMPro.TMP_LineInfo::lineExtents
	Extents_tA2D2F95811D0A18CB7AC3570D2D8F8CD3AF4C4A8 ___lineExtents_19;
};

// UnityEngine.Events.UnityAction`2<UnityEngine.SceneManagement.Scene,UnityEngine.SceneManagement.LoadSceneMode>
struct UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A  : public MulticastDelegate_t
{
};

// UnityEngine.Behaviour
struct Behaviour_t01970CFBBA658497AE30F311C447DB0440BAB7FA  : public Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3
{
};

// System.NotSupportedException
struct NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A  : public SystemException_tCC48D868298F4C0705279823E34B00F4FBDB7295
{
};

// UnityEngine.Renderer
struct Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF  : public Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3
{
};

// UnityEngine.Transform
struct Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1  : public Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3
{
};

// TMPro.WordWrapState
struct WordWrapState_t80F67D8CAA9B1A0A3D5266521E23A9F3100EDD0A 
{
	// System.Int32 TMPro.WordWrapState::previous_WordBreak
	int32_t ___previous_WordBreak_0;
	// System.Int32 TMPro.WordWrapState::total_CharacterCount
	int32_t ___total_CharacterCount_1;
	// System.Int32 TMPro.WordWrapState::visible_CharacterCount
	int32_t ___visible_CharacterCount_2;
	// System.Int32 TMPro.WordWrapState::visible_SpriteCount
	int32_t ___visible_SpriteCount_3;
	// System.Int32 TMPro.WordWrapState::visible_LinkCount
	int32_t ___visible_LinkCount_4;
	// System.Int32 TMPro.WordWrapState::firstCharacterIndex
	int32_t ___firstCharacterIndex_5;
	// System.Int32 TMPro.WordWrapState::firstVisibleCharacterIndex
	int32_t ___firstVisibleCharacterIndex_6;
	// System.Int32 TMPro.WordWrapState::lastCharacterIndex
	int32_t ___lastCharacterIndex_7;
	// System.Int32 TMPro.WordWrapState::lastVisibleCharIndex
	int32_t ___lastVisibleCharIndex_8;
	// System.Int32 TMPro.WordWrapState::lineNumber
	int32_t ___lineNumber_9;
	// System.Single TMPro.WordWrapState::maxCapHeight
	float ___maxCapHeight_10;
	// System.Single TMPro.WordWrapState::maxAscender
	float ___maxAscender_11;
	// System.Single TMPro.WordWrapState::maxDescender
	float ___maxDescender_12;
	// System.Single TMPro.WordWrapState::startOfLineAscender
	float ___startOfLineAscender_13;
	// System.Single TMPro.WordWrapState::maxLineAscender
	float ___maxLineAscender_14;
	// System.Single TMPro.WordWrapState::maxLineDescender
	float ___maxLineDescender_15;
	// System.Single TMPro.WordWrapState::pageAscender
	float ___pageAscender_16;
	// TMPro.HorizontalAlignmentOptions TMPro.WordWrapState::horizontalAlignment
	int32_t ___horizontalAlignment_17;
	// System.Single TMPro.WordWrapState::marginLeft
	float ___marginLeft_18;
	// System.Single TMPro.WordWrapState::marginRight
	float ___marginRight_19;
	// System.Single TMPro.WordWrapState::xAdvance
	float ___xAdvance_20;
	// System.Single TMPro.WordWrapState::preferredWidth
	float ___preferredWidth_21;
	// System.Single TMPro.WordWrapState::preferredHeight
	float ___preferredHeight_22;
	// System.Single TMPro.WordWrapState::previousLineScale
	float ___previousLineScale_23;
	// System.Int32 TMPro.WordWrapState::wordCount
	int32_t ___wordCount_24;
	// TMPro.FontStyles TMPro.WordWrapState::fontStyle
	int32_t ___fontStyle_25;
	// System.Int32 TMPro.WordWrapState::italicAngle
	int32_t ___italicAngle_26;
	// System.Single TMPro.WordWrapState::fontScaleMultiplier
	float ___fontScaleMultiplier_27;
	// System.Single TMPro.WordWrapState::currentFontSize
	float ___currentFontSize_28;
	// System.Single TMPro.WordWrapState::baselineOffset
	float ___baselineOffset_29;
	// System.Single TMPro.WordWrapState::lineOffset
	float ___lineOffset_30;
	// System.Boolean TMPro.WordWrapState::isDrivenLineSpacing
	bool ___isDrivenLineSpacing_31;
	// System.Single TMPro.WordWrapState::glyphHorizontalAdvanceAdjustment
	float ___glyphHorizontalAdvanceAdjustment_32;
	// System.Single TMPro.WordWrapState::cSpace
	float ___cSpace_33;
	// System.Single TMPro.WordWrapState::mSpace
	float ___mSpace_34;
	// TMPro.TMP_TextInfo TMPro.WordWrapState::textInfo
	TMP_TextInfo_t09A8E906329422C3F0C059876801DD695B8D524D* ___textInfo_35;
	// TMPro.TMP_LineInfo TMPro.WordWrapState::lineInfo
	TMP_LineInfo_tB75C1965B58DB7B3A046C8CA55AD6AB92B6B17B3 ___lineInfo_36;
	// UnityEngine.Color32 TMPro.WordWrapState::vertexColor
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___vertexColor_37;
	// UnityEngine.Color32 TMPro.WordWrapState::underlineColor
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___underlineColor_38;
	// UnityEngine.Color32 TMPro.WordWrapState::strikethroughColor
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___strikethroughColor_39;
	// UnityEngine.Color32 TMPro.WordWrapState::highlightColor
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___highlightColor_40;
	// TMPro.TMP_FontStyleStack TMPro.WordWrapState::basicStyleStack
	TMP_FontStyleStack_t52885F172FADBC21346C835B5302167BDA8020DC ___basicStyleStack_41;
	// TMPro.TMP_TextProcessingStack`1<System.Int32> TMPro.WordWrapState::italicAngleStack
	TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C ___italicAngleStack_42;
	// TMPro.TMP_TextProcessingStack`1<UnityEngine.Color32> TMPro.WordWrapState::colorStack
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___colorStack_43;
	// TMPro.TMP_TextProcessingStack`1<UnityEngine.Color32> TMPro.WordWrapState::underlineColorStack
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___underlineColorStack_44;
	// TMPro.TMP_TextProcessingStack`1<UnityEngine.Color32> TMPro.WordWrapState::strikethroughColorStack
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___strikethroughColorStack_45;
	// TMPro.TMP_TextProcessingStack`1<UnityEngine.Color32> TMPro.WordWrapState::highlightColorStack
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___highlightColorStack_46;
	// TMPro.TMP_TextProcessingStack`1<TMPro.HighlightState> TMPro.WordWrapState::highlightStateStack
	TMP_TextProcessingStack_1_t57AECDCC936A7FF1D6CF66CA11560B28A675648D ___highlightStateStack_47;
	// TMPro.TMP_TextProcessingStack`1<TMPro.TMP_ColorGradient> TMPro.WordWrapState::colorGradientStack
	TMP_TextProcessingStack_1_tC8FAEB17246D3B171EFD11165A5761AE39B40D0C ___colorGradientStack_48;
	// TMPro.TMP_TextProcessingStack`1<System.Single> TMPro.WordWrapState::sizeStack
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___sizeStack_49;
	// TMPro.TMP_TextProcessingStack`1<System.Single> TMPro.WordWrapState::indentStack
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___indentStack_50;
	// TMPro.TMP_TextProcessingStack`1<TMPro.FontWeight> TMPro.WordWrapState::fontWeightStack
	TMP_TextProcessingStack_1_tA5C8CED87DD9E73F6359E23B334FFB5B6F813FD4 ___fontWeightStack_51;
	// TMPro.TMP_TextProcessingStack`1<System.Int32> TMPro.WordWrapState::styleStack
	TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C ___styleStack_52;
	// TMPro.TMP_TextProcessingStack`1<System.Single> TMPro.WordWrapState::baselineStack
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___baselineStack_53;
	// TMPro.TMP_TextProcessingStack`1<System.Int32> TMPro.WordWrapState::actionStack
	TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C ___actionStack_54;
	// TMPro.TMP_TextProcessingStack`1<TMPro.MaterialReference> TMPro.WordWrapState::materialReferenceStack
	TMP_TextProcessingStack_1_tB03E08F69415B281A5A81138F09E49EE58402DF9 ___materialReferenceStack_55;
	// TMPro.TMP_TextProcessingStack`1<TMPro.HorizontalAlignmentOptions> TMPro.WordWrapState::lineJustificationStack
	TMP_TextProcessingStack_1_t243EA1B5D7FD2295D6533B953F0BBE8F52EFB8A0 ___lineJustificationStack_56;
	// System.Int32 TMPro.WordWrapState::spriteAnimationID
	int32_t ___spriteAnimationID_57;
	// TMPro.TMP_FontAsset TMPro.WordWrapState::currentFontAsset
	TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160* ___currentFontAsset_58;
	// TMPro.TMP_SpriteAsset TMPro.WordWrapState::currentSpriteAsset
	TMP_SpriteAsset_t81F779E6F705CE190DC0D1F93A954CB8B1774B39* ___currentSpriteAsset_59;
	// UnityEngine.Material TMPro.WordWrapState::currentMaterial
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___currentMaterial_60;
	// System.Int32 TMPro.WordWrapState::currentMaterialIndex
	int32_t ___currentMaterialIndex_61;
	// TMPro.Extents TMPro.WordWrapState::meshExtents
	Extents_tA2D2F95811D0A18CB7AC3570D2D8F8CD3AF4C4A8 ___meshExtents_62;
	// System.Boolean TMPro.WordWrapState::tagNoParsing
	bool ___tagNoParsing_63;
	// System.Boolean TMPro.WordWrapState::isNonBreakingSpace
	bool ___isNonBreakingSpace_64;
};
// Native definition for P/Invoke marshalling of TMPro.WordWrapState
struct WordWrapState_t80F67D8CAA9B1A0A3D5266521E23A9F3100EDD0A_marshaled_pinvoke
{
	int32_t ___previous_WordBreak_0;
	int32_t ___total_CharacterCount_1;
	int32_t ___visible_CharacterCount_2;
	int32_t ___visible_SpriteCount_3;
	int32_t ___visible_LinkCount_4;
	int32_t ___firstCharacterIndex_5;
	int32_t ___firstVisibleCharacterIndex_6;
	int32_t ___lastCharacterIndex_7;
	int32_t ___lastVisibleCharIndex_8;
	int32_t ___lineNumber_9;
	float ___maxCapHeight_10;
	float ___maxAscender_11;
	float ___maxDescender_12;
	float ___startOfLineAscender_13;
	float ___maxLineAscender_14;
	float ___maxLineDescender_15;
	float ___pageAscender_16;
	int32_t ___horizontalAlignment_17;
	float ___marginLeft_18;
	float ___marginRight_19;
	float ___xAdvance_20;
	float ___preferredWidth_21;
	float ___preferredHeight_22;
	float ___previousLineScale_23;
	int32_t ___wordCount_24;
	int32_t ___fontStyle_25;
	int32_t ___italicAngle_26;
	float ___fontScaleMultiplier_27;
	float ___currentFontSize_28;
	float ___baselineOffset_29;
	float ___lineOffset_30;
	int32_t ___isDrivenLineSpacing_31;
	float ___glyphHorizontalAdvanceAdjustment_32;
	float ___cSpace_33;
	float ___mSpace_34;
	TMP_TextInfo_t09A8E906329422C3F0C059876801DD695B8D524D* ___textInfo_35;
	TMP_LineInfo_tB75C1965B58DB7B3A046C8CA55AD6AB92B6B17B3 ___lineInfo_36;
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___vertexColor_37;
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___underlineColor_38;
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___strikethroughColor_39;
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___highlightColor_40;
	TMP_FontStyleStack_t52885F172FADBC21346C835B5302167BDA8020DC ___basicStyleStack_41;
	TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C ___italicAngleStack_42;
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___colorStack_43;
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___underlineColorStack_44;
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___strikethroughColorStack_45;
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___highlightColorStack_46;
	TMP_TextProcessingStack_1_t57AECDCC936A7FF1D6CF66CA11560B28A675648D ___highlightStateStack_47;
	TMP_TextProcessingStack_1_tC8FAEB17246D3B171EFD11165A5761AE39B40D0C ___colorGradientStack_48;
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___sizeStack_49;
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___indentStack_50;
	TMP_TextProcessingStack_1_tA5C8CED87DD9E73F6359E23B334FFB5B6F813FD4 ___fontWeightStack_51;
	TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C ___styleStack_52;
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___baselineStack_53;
	TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C ___actionStack_54;
	TMP_TextProcessingStack_1_tB03E08F69415B281A5A81138F09E49EE58402DF9 ___materialReferenceStack_55;
	TMP_TextProcessingStack_1_t243EA1B5D7FD2295D6533B953F0BBE8F52EFB8A0 ___lineJustificationStack_56;
	int32_t ___spriteAnimationID_57;
	TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160* ___currentFontAsset_58;
	TMP_SpriteAsset_t81F779E6F705CE190DC0D1F93A954CB8B1774B39* ___currentSpriteAsset_59;
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___currentMaterial_60;
	int32_t ___currentMaterialIndex_61;
	Extents_tA2D2F95811D0A18CB7AC3570D2D8F8CD3AF4C4A8 ___meshExtents_62;
	int32_t ___tagNoParsing_63;
	int32_t ___isNonBreakingSpace_64;
};
// Native definition for COM marshalling of TMPro.WordWrapState
struct WordWrapState_t80F67D8CAA9B1A0A3D5266521E23A9F3100EDD0A_marshaled_com
{
	int32_t ___previous_WordBreak_0;
	int32_t ___total_CharacterCount_1;
	int32_t ___visible_CharacterCount_2;
	int32_t ___visible_SpriteCount_3;
	int32_t ___visible_LinkCount_4;
	int32_t ___firstCharacterIndex_5;
	int32_t ___firstVisibleCharacterIndex_6;
	int32_t ___lastCharacterIndex_7;
	int32_t ___lastVisibleCharIndex_8;
	int32_t ___lineNumber_9;
	float ___maxCapHeight_10;
	float ___maxAscender_11;
	float ___maxDescender_12;
	float ___startOfLineAscender_13;
	float ___maxLineAscender_14;
	float ___maxLineDescender_15;
	float ___pageAscender_16;
	int32_t ___horizontalAlignment_17;
	float ___marginLeft_18;
	float ___marginRight_19;
	float ___xAdvance_20;
	float ___preferredWidth_21;
	float ___preferredHeight_22;
	float ___previousLineScale_23;
	int32_t ___wordCount_24;
	int32_t ___fontStyle_25;
	int32_t ___italicAngle_26;
	float ___fontScaleMultiplier_27;
	float ___currentFontSize_28;
	float ___baselineOffset_29;
	float ___lineOffset_30;
	int32_t ___isDrivenLineSpacing_31;
	float ___glyphHorizontalAdvanceAdjustment_32;
	float ___cSpace_33;
	float ___mSpace_34;
	TMP_TextInfo_t09A8E906329422C3F0C059876801DD695B8D524D* ___textInfo_35;
	TMP_LineInfo_tB75C1965B58DB7B3A046C8CA55AD6AB92B6B17B3 ___lineInfo_36;
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___vertexColor_37;
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___underlineColor_38;
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___strikethroughColor_39;
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___highlightColor_40;
	TMP_FontStyleStack_t52885F172FADBC21346C835B5302167BDA8020DC ___basicStyleStack_41;
	TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C ___italicAngleStack_42;
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___colorStack_43;
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___underlineColorStack_44;
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___strikethroughColorStack_45;
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___highlightColorStack_46;
	TMP_TextProcessingStack_1_t57AECDCC936A7FF1D6CF66CA11560B28A675648D ___highlightStateStack_47;
	TMP_TextProcessingStack_1_tC8FAEB17246D3B171EFD11165A5761AE39B40D0C ___colorGradientStack_48;
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___sizeStack_49;
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___indentStack_50;
	TMP_TextProcessingStack_1_tA5C8CED87DD9E73F6359E23B334FFB5B6F813FD4 ___fontWeightStack_51;
	TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C ___styleStack_52;
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___baselineStack_53;
	TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C ___actionStack_54;
	TMP_TextProcessingStack_1_tB03E08F69415B281A5A81138F09E49EE58402DF9 ___materialReferenceStack_55;
	TMP_TextProcessingStack_1_t243EA1B5D7FD2295D6533B953F0BBE8F52EFB8A0 ___lineJustificationStack_56;
	int32_t ___spriteAnimationID_57;
	TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160* ___currentFontAsset_58;
	TMP_SpriteAsset_t81F779E6F705CE190DC0D1F93A954CB8B1774B39* ___currentSpriteAsset_59;
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___currentMaterial_60;
	int32_t ___currentMaterialIndex_61;
	Extents_tA2D2F95811D0A18CB7AC3570D2D8F8CD3AF4C4A8 ___meshExtents_62;
	int32_t ___tagNoParsing_63;
	int32_t ___isNonBreakingSpace_64;
};

// TMPro.TMP_TextProcessingStack`1<TMPro.WordWrapState>
struct TMP_TextProcessingStack_1_t2DDA00FFC64AF6E3AFD475AB2086D16C34787E0F 
{
	// T[] TMPro.TMP_TextProcessingStack`1::itemStack
	WordWrapStateU5BU5D_t473D59C9DBCC949CE72EF1EB471CBA152A6CEAC9* ___itemStack_0;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::index
	int32_t ___index_1;
	// T TMPro.TMP_TextProcessingStack`1::m_DefaultItem
	WordWrapState_t80F67D8CAA9B1A0A3D5266521E23A9F3100EDD0A ___m_DefaultItem_2;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Capacity
	int32_t ___m_Capacity_3;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_RolloverSize
	int32_t ___m_RolloverSize_4;
	// System.Int32 TMPro.TMP_TextProcessingStack`1::m_Count
	int32_t ___m_Count_5;
};

// UnityEngine.Canvas
struct Canvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26  : public Behaviour_t01970CFBBA658497AE30F311C447DB0440BAB7FA
{
};

struct Canvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26_StaticFields
{
	// UnityEngine.Canvas/WillRenderCanvases UnityEngine.Canvas::preWillRenderCanvases
	WillRenderCanvases_tA4A6E66DBA797DCB45B995DBA449A9D1D80D0FBC* ___preWillRenderCanvases_4;
	// UnityEngine.Canvas/WillRenderCanvases UnityEngine.Canvas::willRenderCanvases
	WillRenderCanvases_tA4A6E66DBA797DCB45B995DBA449A9D1D80D0FBC* ___willRenderCanvases_5;
	// System.Action`1<System.Int32> UnityEngine.Canvas::<externBeginRenderOverlays>k__BackingField
	Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* ___U3CexternBeginRenderOverlaysU3Ek__BackingField_6;
	// System.Action`2<System.Int32,System.Int32> UnityEngine.Canvas::<externRenderOverlaysBefore>k__BackingField
	Action_2_tD7438462601D3939500ED67463331FE00CFFBDB8* ___U3CexternRenderOverlaysBeforeU3Ek__BackingField_7;
	// System.Action`1<System.Int32> UnityEngine.Canvas::<externEndRenderOverlays>k__BackingField
	Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* ___U3CexternEndRenderOverlaysU3Ek__BackingField_8;
};

// UnityEngine.CanvasGroup
struct CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094  : public Behaviour_t01970CFBBA658497AE30F311C447DB0440BAB7FA
{
};

// UnityEngine.MonoBehaviour
struct MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71  : public Behaviour_t01970CFBBA658497AE30F311C447DB0440BAB7FA
{
};

// UnityEngine.SpriteRenderer
struct SpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B  : public Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF
{
	// UnityEngine.Events.UnityEvent`1<UnityEngine.SpriteRenderer> UnityEngine.SpriteRenderer::m_SpriteChangeEvent
	UnityEvent_1_t8ABE5544759145B8D7A09F1C54FFCB6907EDD56E* ___m_SpriteChangeEvent_4;
};

// Dice
struct Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
	// System.Int32 Dice::diceNum
	int32_t ___diceNum_4;
	// System.String Dice::diceType
	String_t* ___diceType_5;
	// System.String Dice::statAddedTo
	String_t* ___statAddedTo_6;
	// System.Boolean Dice::moveable
	bool ___moveable_7;
	// System.Boolean Dice::isAttached
	bool ___isAttached_8;
	// System.Boolean Dice::isRerolled
	bool ___isRerolled_9;
	// System.String Dice::isOnPlayerOrEnemy
	String_t* ___isOnPlayerOrEnemy_10;
	// UnityEngine.Vector3 Dice::instantiationPos
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___instantiationPos_11;
	// UnityEngine.SpriteRenderer Dice::spriteRenderer
	SpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B* ___spriteRenderer_12;
	// UnityEngine.SpriteRenderer Dice::childSpriteRenderer
	SpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B* ___childSpriteRenderer_13;
	// Scripts Dice::scripts
	Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* ___scripts_14;
	// UnityEngine.WaitForSeconds[] Dice::rollTimes
	WaitForSecondsU5BU5D_t2A9038ECB6E643745AEF2AD9A4F7FFD3D272186E* ___rollTimes_15;
};

// DiceSummoner
struct DiceSummoner_tA51E21ADF511DD948DD0B44C50A73CF2F8C29DBC  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
	// UnityEngine.GameObject DiceSummoner::diceBase
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___diceBase_4;
	// UnityEngine.GameObject[] DiceSummoner::numArr
	GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* ___numArr_5;
	// System.Boolean DiceSummoner::breakOutOfScimitarParryLoop
	bool ___breakOutOfScimitarParryLoop_6;
	// Scripts DiceSummoner::scripts
	Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* ___scripts_7;
	// System.Collections.Generic.List`1<UnityEngine.GameObject> DiceSummoner::existingDice
	List_1_tB951CE80B58D1BF9650862451D8DAD8C231F207B* ___existingDice_8;
	// System.Single DiceSummoner::yCoord
	float ___yCoord_9;
	// System.Single[] DiceSummoner::xCoords
	SingleU5BU5D_t89DEFE97BCEDB5857010E79ECE0F52CF6E93B87C* ___xCoords_10;
	// System.Collections.Generic.List`1<UnityEngine.Color> DiceSummoner::generatedTypes
	List_1_t242CDEAEC9C92000DA96982CDB9D592DDE2AADAF* ___generatedTypes_11;
	// System.Int32 DiceSummoner::lastNum
	int32_t ___lastNum_12;
	// System.String DiceSummoner::lastType
	String_t* ___lastType_13;
	// System.String DiceSummoner::lastStat
	String_t* ___lastStat_14;
};

// Fader
struct Fader_t4082384C0679E40CABDB8F1A51E0246989537D24  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
	// System.Boolean Fader::start
	bool ___start_4;
	// System.Single Fader::fadeDamp
	float ___fadeDamp_5;
	// System.String Fader::fadeScene
	String_t* ___fadeScene_6;
	// System.Single Fader::alpha
	float ___alpha_7;
	// UnityEngine.Color Fader::fadeColor
	Color_tD001788D726C3A7F1379BEED0260B9591F440C1F ___fadeColor_8;
	// System.Boolean Fader::isFadeIn
	bool ___isFadeIn_9;
	// UnityEngine.CanvasGroup Fader::myCanvas
	CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094* ___myCanvas_10;
	// UnityEngine.UI.Image Fader::bg
	Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* ___bg_11;
	// System.Single Fader::lastTime
	float ___lastTime_12;
	// System.Boolean Fader::startedLoading
	bool ___startedLoading_13;
};

// Scripts
struct Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
	// UnityEngine.Animator Scripts::terrain
	Animator_t8A52E42AE54F76681838FE9E632683EF3952E883* ___terrain_4;
	// Dice Scripts::dice
	Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* ___dice_5;
	// Arrow Scripts::arrow
	Arrow_t7048A6830A9F76E0448BBD44FD9C3C00BC138DBF* ___arrow_6;
	// Enemy Scripts::enemy
	Enemy_t10DB314C96B1CE78B8D967CD3B39F05126409BBB* ___enemy_7;
	// Music Scripts::music
	Music_t95D6293158A4741467B5F53F0E61597A72226ECD* ___music_8;
	// Colors Scripts::colors
	Colors_t4FBC8F9BC3173CFB15C6164CC275EEBAC6E0E973* ___colors_9;
	// Player Scripts::player
	Player_tF98BD09D3495D2FF1922E5D34866AEAC6AE2DF74* ___player_10;
	// MenuIcon Scripts::menuIcon
	MenuIcon_tC1941FC04C0252157C37212443EB28203FBE0EA9* ___menuIcon_11;
	// Tutorial Scripts::tutorial
	Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* ___tutorial_12;
	// BackToMenu Scripts::backToMenu
	BackToMenu_tD8E3B8BA9822D00CBD4BA804F41B32438B39E4BC* ___backToMenu_13;
	// MenuButton Scripts::menuButton
	MenuButton_t5FBBD57E4378A9FCDB747615454A243C197C42A2* ___menuButton_14;
	// Statistics Scripts::statistics
	Statistics_t189B79C95098317A18A30BF4120F43A59ABB6431* ___statistics_15;
	// TurnManager Scripts::turnManager
	TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* ___turnManager_16;
	// ItemManager Scripts::itemManager
	ItemManager_t396CBB91EABD1E73BA16A4DBE2E89AC1601FB83E* ___itemManager_17;
	// DiceSummoner Scripts::diceSummoner
	DiceSummoner_tA51E21ADF511DD948DD0B44C50A73CF2F8C29DBC* ___diceSummoner_18;
	// SoundManager Scripts::soundManager
	SoundManager_tCA2CCAC5CDF1BA10E525C01C8D1D0DFAC9BE3734* ___soundManager_19;
	// LevelManager Scripts::levelManager
	LevelManager_t8405886BBC5A0ACBB1CC210E25D5DA1C72D16530* ___levelManager_20;
	// StatSummoner Scripts::statSummoner
	StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539* ___statSummoner_21;
	// TombstoneData Scripts::tombstoneData
	TombstoneData_t98D1E2F7C78F45B3AE4D2A37D2FF480FFC3F7CC5* ___tombstoneData_22;
	// CharacterSelector Scripts::characterSelector
	CharacterSelector_t3FDA33FAF8CF21DF9FC1E34A558DA73D2FACB64E* ___characterSelector_23;
	// HighlightCalculator Scripts::highlightCalculator
	HighlightCalculator_t9A040BB70BE3C30320C9B31D3A60280A9D27E9B6* ___highlightCalculator_24;
	// System.Single[] Scripts::delayArr
	SingleU5BU5D_t89DEFE97BCEDB5857010E79ECE0F52CF6E93B87C* ___delayArr_25;
	// System.Collections.Generic.Dictionary`2<System.Single,UnityEngine.WaitForSeconds> Scripts::delays
	Dictionary_2_t5826E39B66190EFF2D3EA81361D5FC4BB8D6B212* ___delays_26;
	// System.String Scripts::DEBUG_KEY
	String_t* ___DEBUG_KEY_27;
	// System.String Scripts::HINTS_KEY
	String_t* ___HINTS_KEY_28;
	// System.String Scripts::SOUNDS_KEY
	String_t* ___SOUNDS_KEY_29;
	// System.String Scripts::MUSIC_KEY
	String_t* ___MUSIC_KEY_30;
	// System.String Scripts::BUTTONS_KEY
	String_t* ___BUTTONS_KEY_31;
};

// SoundManager
struct SoundManager_tCA2CCAC5CDF1BA10E525C01C8D1D0DFAC9BE3734  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
	// UnityEngine.AudioClip[] SoundManager::audioClips
	AudioClipU5BU5D_t916722468F7EDCFA833318C35CD7D41097D75D31* ___audioClips_4;
	// System.String[] SoundManager::audioClipNames
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___audioClipNames_5;
	// UnityEngine.AudioSource SoundManager::audioSource
	AudioSource_t871AC2272F896738252F04EE949AEF5B241D3299* ___audioSource_6;
	// Scripts SoundManager::scripts
	Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* ___scripts_7;
};

// StatSummoner
struct StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
	// UnityEngine.GameObject StatSummoner::plus
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___plus_4;
	// UnityEngine.GameObject StatSummoner::minus
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___minus_5;
	// UnityEngine.GameObject StatSummoner::square
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___square_6;
	// UnityEngine.GameObject StatSummoner::negSquare
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___negSquare_7;
	// UnityEngine.GameObject StatSummoner::circle
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___circle_8;
	// TMPro.TextMeshProUGUI StatSummoner::playerDebug
	TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* ___playerDebug_9;
	// TMPro.TextMeshProUGUI StatSummoner::enemyDebug
	TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* ___enemyDebug_10;
	// System.Single StatSummoner::xCoord
	float ___xCoord_11;
	// System.Single StatSummoner::xOffset
	float ___xOffset_12;
	// System.Single StatSummoner::highlightOffset
	float ___highlightOffset_13;
	// System.Single StatSummoner::diceOffset
	float ___diceOffset_14;
	// System.Single StatSummoner::buttonXCoord
	float ___buttonXCoord_15;
	// System.Single StatSummoner::buttonXOffset
	float ___buttonXOffset_16;
	// System.Single[] StatSummoner::yCoords
	SingleU5BU5D_t89DEFE97BCEDB5857010E79ECE0F52CF6E93B87C* ___yCoords_17;
	// UnityEngine.Vector2 StatSummoner::baseDebugPos
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___baseDebugPos_18;
	// System.Collections.Generic.List`1<UnityEngine.GameObject> StatSummoner::existingStatSquares
	List_1_tB951CE80B58D1BF9650862451D8DAD8C231F207B* ___existingStatSquares_19;
	// System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.List`1<Dice>> StatSummoner::addedPlayerDice
	Dictionary_2_t71DC4111D9530C502ACEA7A17068D6DC7AF41689* ___addedPlayerDice_20;
	// System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.List`1<Dice>> StatSummoner::addedEnemyDice
	Dictionary_2_t71DC4111D9530C502ACEA7A17068D6DC7AF41689* ___addedEnemyDice_21;
	// System.Collections.Generic.Dictionary`2<System.String,System.Int32> StatSummoner::addedPlayerStamina
	Dictionary_2_t5C8F46F5D57502270DD9E1DA8303B23C7FE85588* ___addedPlayerStamina_22;
	// System.Collections.Generic.Dictionary`2<System.String,System.Int32> StatSummoner::addedEnemyStamina
	Dictionary_2_t5C8F46F5D57502270DD9E1DA8303B23C7FE85588* ___addedEnemyStamina_23;
	// Scripts StatSummoner::scripts
	Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* ___scripts_24;
};

// TurnManager
struct TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
	// UnityEngine.GameObject TurnManager::blackBox
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___blackBox_4;
	// UnityEngine.Vector3 TurnManager::onScreen
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___onScreen_5;
	// UnityEngine.Vector3 TurnManager::offScreen
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___offScreen_6;
	// UnityEngine.Coroutine TurnManager::coroutine
	Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* ___coroutine_7;
	// TMPro.TextMeshProUGUI TurnManager::statusText
	TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* ___statusText_8;
	// System.String[] TurnManager::targetArr
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___targetArr_9;
	// System.String[] TurnManager::targetInfoArr
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___targetInfoArr_10;
	// Scripts TurnManager::scripts
	Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* ___scripts_11;
	// System.Boolean TurnManager::isMoving
	bool ___isMoving_12;
	// System.Boolean TurnManager::actionsAvailable
	bool ___actionsAvailable_13;
	// System.Boolean TurnManager::alterationDuringMove
	bool ___alterationDuringMove_14;
	// System.Boolean TurnManager::scimitarParry
	bool ___scimitarParry_15;
	// UnityEngine.GameObject TurnManager::dieSavedFromLastRound
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___dieSavedFromLastRound_16;
	// System.Boolean TurnManager::discardDieBecauseCourage
	bool ___discardDieBecauseCourage_17;
	// System.Boolean TurnManager::dontRemoveLeechYet
	bool ___dontRemoveLeechYet_18;
};

// Tutorial
struct Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
	// UnityEngine.Sprite Tutorial::blackBox
	Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* ___blackBox_4;
	// TMPro.TextMeshProUGUI Tutorial::tutorialText
	TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* ___tutorialText_5;
	// TMPro.TextMeshProUGUI Tutorial::statText
	TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* ___statText_6;
	// System.Int32 Tutorial::curIndex
	int32_t ___curIndex_7;
	// Scripts Tutorial::scripts
	Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* ___scripts_8;
	// System.Boolean Tutorial::preventAttack
	bool ___preventAttack_9;
	// System.Boolean Tutorial::isAnimating
	bool ___isAnimating_10;
	// UnityEngine.Coroutine Tutorial::mainScroll
	Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* ___mainScroll_11;
	// UnityEngine.Coroutine Tutorial::statScroll
	Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* ___statScroll_12;
	// System.Collections.Generic.List`1<System.String> Tutorial::tutorialTextList
	List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* ___tutorialTextList_13;
};

// UnityEngine.EventSystems.UIBehaviour
struct UIBehaviour_tB9D4295827BD2EEDEF0749200C6CA7090C742A9D  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
};

// UnityEngine.UI.Graphic
struct Graphic_tCBFCA4585A19E2B75465AECFEAC43F4016BF7931  : public UIBehaviour_tB9D4295827BD2EEDEF0749200C6CA7090C742A9D
{
	// UnityEngine.Material UnityEngine.UI.Graphic::m_Material
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___m_Material_6;
	// UnityEngine.Color UnityEngine.UI.Graphic::m_Color
	Color_tD001788D726C3A7F1379BEED0260B9591F440C1F ___m_Color_7;
	// System.Boolean UnityEngine.UI.Graphic::m_SkipLayoutUpdate
	bool ___m_SkipLayoutUpdate_8;
	// System.Boolean UnityEngine.UI.Graphic::m_SkipMaterialUpdate
	bool ___m_SkipMaterialUpdate_9;
	// System.Boolean UnityEngine.UI.Graphic::m_RaycastTarget
	bool ___m_RaycastTarget_10;
	// UnityEngine.Vector4 UnityEngine.UI.Graphic::m_RaycastPadding
	Vector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3 ___m_RaycastPadding_11;
	// UnityEngine.RectTransform UnityEngine.UI.Graphic::m_RectTransform
	RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* ___m_RectTransform_12;
	// UnityEngine.CanvasRenderer UnityEngine.UI.Graphic::m_CanvasRenderer
	CanvasRenderer_tAB9A55A976C4E3B2B37D0CE5616E5685A8B43860* ___m_CanvasRenderer_13;
	// UnityEngine.Canvas UnityEngine.UI.Graphic::m_Canvas
	Canvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26* ___m_Canvas_14;
	// System.Boolean UnityEngine.UI.Graphic::m_VertsDirty
	bool ___m_VertsDirty_15;
	// System.Boolean UnityEngine.UI.Graphic::m_MaterialDirty
	bool ___m_MaterialDirty_16;
	// UnityEngine.Events.UnityAction UnityEngine.UI.Graphic::m_OnDirtyLayoutCallback
	UnityAction_t11A1F3B953B365C072A5DCC32677EE1796A962A7* ___m_OnDirtyLayoutCallback_17;
	// UnityEngine.Events.UnityAction UnityEngine.UI.Graphic::m_OnDirtyVertsCallback
	UnityAction_t11A1F3B953B365C072A5DCC32677EE1796A962A7* ___m_OnDirtyVertsCallback_18;
	// UnityEngine.Events.UnityAction UnityEngine.UI.Graphic::m_OnDirtyMaterialCallback
	UnityAction_t11A1F3B953B365C072A5DCC32677EE1796A962A7* ___m_OnDirtyMaterialCallback_19;
	// UnityEngine.Mesh UnityEngine.UI.Graphic::m_CachedMesh
	Mesh_t6D9C539763A09BC2B12AEAEF36F6DFFC98AE63D4* ___m_CachedMesh_22;
	// UnityEngine.Vector2[] UnityEngine.UI.Graphic::m_CachedUvs
	Vector2U5BU5D_tFEBBC94BCC6C9C88277BA04047D2B3FDB6ED7FDA* ___m_CachedUvs_23;
	// UnityEngine.UI.CoroutineTween.TweenRunner`1<UnityEngine.UI.CoroutineTween.ColorTween> UnityEngine.UI.Graphic::m_ColorTweenRunner
	TweenRunner_1_t5BB0582F926E75E2FE795492679A6CF55A4B4BC4* ___m_ColorTweenRunner_24;
	// System.Boolean UnityEngine.UI.Graphic::<useLegacyMeshGeneration>k__BackingField
	bool ___U3CuseLegacyMeshGenerationU3Ek__BackingField_25;
};

struct Graphic_tCBFCA4585A19E2B75465AECFEAC43F4016BF7931_StaticFields
{
	// UnityEngine.Material UnityEngine.UI.Graphic::s_DefaultUI
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___s_DefaultUI_4;
	// UnityEngine.Texture2D UnityEngine.UI.Graphic::s_WhiteTexture
	Texture2D_tE6505BC111DD8A424A9DBE8E05D7D09E11FFFCF4* ___s_WhiteTexture_5;
	// UnityEngine.Mesh UnityEngine.UI.Graphic::s_Mesh
	Mesh_t6D9C539763A09BC2B12AEAEF36F6DFFC98AE63D4* ___s_Mesh_20;
	// UnityEngine.UI.VertexHelper UnityEngine.UI.Graphic::s_VertexHelper
	VertexHelper_tB905FCB02AE67CBEE5F265FE37A5938FC5D136FE* ___s_VertexHelper_21;
};

// UnityEngine.UI.MaskableGraphic
struct MaskableGraphic_tFC5B6BE351C90DE53744DF2A70940242774B361E  : public Graphic_tCBFCA4585A19E2B75465AECFEAC43F4016BF7931
{
	// System.Boolean UnityEngine.UI.MaskableGraphic::m_ShouldRecalculateStencil
	bool ___m_ShouldRecalculateStencil_26;
	// UnityEngine.Material UnityEngine.UI.MaskableGraphic::m_MaskMaterial
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___m_MaskMaterial_27;
	// UnityEngine.UI.RectMask2D UnityEngine.UI.MaskableGraphic::m_ParentMask
	RectMask2D_tACF92BE999C791A665BD1ADEABF5BCEB82846670* ___m_ParentMask_28;
	// System.Boolean UnityEngine.UI.MaskableGraphic::m_Maskable
	bool ___m_Maskable_29;
	// System.Boolean UnityEngine.UI.MaskableGraphic::m_IsMaskingGraphic
	bool ___m_IsMaskingGraphic_30;
	// System.Boolean UnityEngine.UI.MaskableGraphic::m_IncludeForMasking
	bool ___m_IncludeForMasking_31;
	// UnityEngine.UI.MaskableGraphic/CullStateChangedEvent UnityEngine.UI.MaskableGraphic::m_OnCullStateChanged
	CullStateChangedEvent_t6073CD0D951EC1256BF74B8F9107D68FC89B99B8* ___m_OnCullStateChanged_32;
	// System.Boolean UnityEngine.UI.MaskableGraphic::m_ShouldRecalculate
	bool ___m_ShouldRecalculate_33;
	// System.Int32 UnityEngine.UI.MaskableGraphic::m_StencilValue
	int32_t ___m_StencilValue_34;
	// UnityEngine.Vector3[] UnityEngine.UI.MaskableGraphic::m_Corners
	Vector3U5BU5D_tFF1859CCE176131B909E2044F76443064254679C* ___m_Corners_35;
};

// UnityEngine.UI.Image
struct Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E  : public MaskableGraphic_tFC5B6BE351C90DE53744DF2A70940242774B361E
{
	// UnityEngine.Sprite UnityEngine.UI.Image::m_Sprite
	Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* ___m_Sprite_37;
	// UnityEngine.Sprite UnityEngine.UI.Image::m_OverrideSprite
	Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* ___m_OverrideSprite_38;
	// UnityEngine.UI.Image/Type UnityEngine.UI.Image::m_Type
	int32_t ___m_Type_39;
	// System.Boolean UnityEngine.UI.Image::m_PreserveAspect
	bool ___m_PreserveAspect_40;
	// System.Boolean UnityEngine.UI.Image::m_FillCenter
	bool ___m_FillCenter_41;
	// UnityEngine.UI.Image/FillMethod UnityEngine.UI.Image::m_FillMethod
	int32_t ___m_FillMethod_42;
	// System.Single UnityEngine.UI.Image::m_FillAmount
	float ___m_FillAmount_43;
	// System.Boolean UnityEngine.UI.Image::m_FillClockwise
	bool ___m_FillClockwise_44;
	// System.Int32 UnityEngine.UI.Image::m_FillOrigin
	int32_t ___m_FillOrigin_45;
	// System.Single UnityEngine.UI.Image::m_AlphaHitTestMinimumThreshold
	float ___m_AlphaHitTestMinimumThreshold_46;
	// System.Boolean UnityEngine.UI.Image::m_Tracked
	bool ___m_Tracked_47;
	// System.Boolean UnityEngine.UI.Image::m_UseSpriteMesh
	bool ___m_UseSpriteMesh_48;
	// System.Single UnityEngine.UI.Image::m_PixelsPerUnitMultiplier
	float ___m_PixelsPerUnitMultiplier_49;
	// System.Single UnityEngine.UI.Image::m_CachedReferencePixelsPerUnit
	float ___m_CachedReferencePixelsPerUnit_50;
};

struct Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_StaticFields
{
	// UnityEngine.Material UnityEngine.UI.Image::s_ETC1DefaultUI
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___s_ETC1DefaultUI_36;
	// UnityEngine.Vector2[] UnityEngine.UI.Image::s_VertScratch
	Vector2U5BU5D_tFEBBC94BCC6C9C88277BA04047D2B3FDB6ED7FDA* ___s_VertScratch_51;
	// UnityEngine.Vector2[] UnityEngine.UI.Image::s_UVScratch
	Vector2U5BU5D_tFEBBC94BCC6C9C88277BA04047D2B3FDB6ED7FDA* ___s_UVScratch_52;
	// UnityEngine.Vector3[] UnityEngine.UI.Image::s_Xy
	Vector3U5BU5D_tFF1859CCE176131B909E2044F76443064254679C* ___s_Xy_53;
	// UnityEngine.Vector3[] UnityEngine.UI.Image::s_Uv
	Vector3U5BU5D_tFF1859CCE176131B909E2044F76443064254679C* ___s_Uv_54;
	// System.Collections.Generic.List`1<UnityEngine.UI.Image> UnityEngine.UI.Image::m_TrackedTexturelessImages
	List_1_tE6BB71ABF15905EFA2BE92C38A2716547AEADB19* ___m_TrackedTexturelessImages_55;
	// System.Boolean UnityEngine.UI.Image::s_Initialized
	bool ___s_Initialized_56;
};

// TMPro.TMP_Text
struct TMP_Text_tE8D677872D43AD4B2AAF0D6101692A17D0B251A9  : public MaskableGraphic_tFC5B6BE351C90DE53744DF2A70940242774B361E
{
	// System.String TMPro.TMP_Text::m_text
	String_t* ___m_text_36;
	// System.Boolean TMPro.TMP_Text::m_IsTextBackingStringDirty
	bool ___m_IsTextBackingStringDirty_37;
	// TMPro.ITextPreprocessor TMPro.TMP_Text::m_TextPreprocessor
	RuntimeObject* ___m_TextPreprocessor_38;
	// System.Boolean TMPro.TMP_Text::m_isRightToLeft
	bool ___m_isRightToLeft_39;
	// TMPro.TMP_FontAsset TMPro.TMP_Text::m_fontAsset
	TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160* ___m_fontAsset_40;
	// TMPro.TMP_FontAsset TMPro.TMP_Text::m_currentFontAsset
	TMP_FontAsset_t923BF2F78D7C5AC36376E168A1193B7CB4855160* ___m_currentFontAsset_41;
	// System.Boolean TMPro.TMP_Text::m_isSDFShader
	bool ___m_isSDFShader_42;
	// UnityEngine.Material TMPro.TMP_Text::m_sharedMaterial
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___m_sharedMaterial_43;
	// UnityEngine.Material TMPro.TMP_Text::m_currentMaterial
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___m_currentMaterial_44;
	// System.Int32 TMPro.TMP_Text::m_currentMaterialIndex
	int32_t ___m_currentMaterialIndex_48;
	// UnityEngine.Material[] TMPro.TMP_Text::m_fontSharedMaterials
	MaterialU5BU5D_t2B1D11C42DB07A4400C0535F92DBB87A2E346D3D* ___m_fontSharedMaterials_49;
	// UnityEngine.Material TMPro.TMP_Text::m_fontMaterial
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___m_fontMaterial_50;
	// UnityEngine.Material[] TMPro.TMP_Text::m_fontMaterials
	MaterialU5BU5D_t2B1D11C42DB07A4400C0535F92DBB87A2E346D3D* ___m_fontMaterials_51;
	// System.Boolean TMPro.TMP_Text::m_isMaterialDirty
	bool ___m_isMaterialDirty_52;
	// UnityEngine.Color32 TMPro.TMP_Text::m_fontColor32
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___m_fontColor32_53;
	// UnityEngine.Color TMPro.TMP_Text::m_fontColor
	Color_tD001788D726C3A7F1379BEED0260B9591F440C1F ___m_fontColor_54;
	// UnityEngine.Color32 TMPro.TMP_Text::m_underlineColor
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___m_underlineColor_56;
	// UnityEngine.Color32 TMPro.TMP_Text::m_strikethroughColor
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___m_strikethroughColor_57;
	// System.Boolean TMPro.TMP_Text::m_enableVertexGradient
	bool ___m_enableVertexGradient_58;
	// TMPro.ColorMode TMPro.TMP_Text::m_colorMode
	int32_t ___m_colorMode_59;
	// TMPro.VertexGradient TMPro.TMP_Text::m_fontColorGradient
	VertexGradient_t2C057B53C0EA6E987C2B7BAB0305E686DA1C9A8F ___m_fontColorGradient_60;
	// TMPro.TMP_ColorGradient TMPro.TMP_Text::m_fontColorGradientPreset
	TMP_ColorGradient_t17B51752B4E9499A1FF7D875DCEC1D15A0F4AEBB* ___m_fontColorGradientPreset_61;
	// TMPro.TMP_SpriteAsset TMPro.TMP_Text::m_spriteAsset
	TMP_SpriteAsset_t81F779E6F705CE190DC0D1F93A954CB8B1774B39* ___m_spriteAsset_62;
	// System.Boolean TMPro.TMP_Text::m_tintAllSprites
	bool ___m_tintAllSprites_63;
	// System.Boolean TMPro.TMP_Text::m_tintSprite
	bool ___m_tintSprite_64;
	// UnityEngine.Color32 TMPro.TMP_Text::m_spriteColor
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___m_spriteColor_65;
	// TMPro.TMP_StyleSheet TMPro.TMP_Text::m_StyleSheet
	TMP_StyleSheet_t70C71699F5CB2D855C361DBB78A44C901236C859* ___m_StyleSheet_66;
	// TMPro.TMP_Style TMPro.TMP_Text::m_TextStyle
	TMP_Style_tA9E5B1B35EBFE24EF980CEA03251B638282E120C* ___m_TextStyle_67;
	// System.Int32 TMPro.TMP_Text::m_TextStyleHashCode
	int32_t ___m_TextStyleHashCode_68;
	// System.Boolean TMPro.TMP_Text::m_overrideHtmlColors
	bool ___m_overrideHtmlColors_69;
	// UnityEngine.Color32 TMPro.TMP_Text::m_faceColor
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___m_faceColor_70;
	// UnityEngine.Color32 TMPro.TMP_Text::m_outlineColor
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___m_outlineColor_71;
	// System.Single TMPro.TMP_Text::m_outlineWidth
	float ___m_outlineWidth_72;
	// System.Single TMPro.TMP_Text::m_fontSize
	float ___m_fontSize_73;
	// System.Single TMPro.TMP_Text::m_currentFontSize
	float ___m_currentFontSize_74;
	// System.Single TMPro.TMP_Text::m_fontSizeBase
	float ___m_fontSizeBase_75;
	// TMPro.TMP_TextProcessingStack`1<System.Single> TMPro.TMP_Text::m_sizeStack
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___m_sizeStack_76;
	// TMPro.FontWeight TMPro.TMP_Text::m_fontWeight
	int32_t ___m_fontWeight_77;
	// TMPro.FontWeight TMPro.TMP_Text::m_FontWeightInternal
	int32_t ___m_FontWeightInternal_78;
	// TMPro.TMP_TextProcessingStack`1<TMPro.FontWeight> TMPro.TMP_Text::m_FontWeightStack
	TMP_TextProcessingStack_1_tA5C8CED87DD9E73F6359E23B334FFB5B6F813FD4 ___m_FontWeightStack_79;
	// System.Boolean TMPro.TMP_Text::m_enableAutoSizing
	bool ___m_enableAutoSizing_80;
	// System.Single TMPro.TMP_Text::m_maxFontSize
	float ___m_maxFontSize_81;
	// System.Single TMPro.TMP_Text::m_minFontSize
	float ___m_minFontSize_82;
	// System.Int32 TMPro.TMP_Text::m_AutoSizeIterationCount
	int32_t ___m_AutoSizeIterationCount_83;
	// System.Int32 TMPro.TMP_Text::m_AutoSizeMaxIterationCount
	int32_t ___m_AutoSizeMaxIterationCount_84;
	// System.Boolean TMPro.TMP_Text::m_IsAutoSizePointSizeSet
	bool ___m_IsAutoSizePointSizeSet_85;
	// System.Single TMPro.TMP_Text::m_fontSizeMin
	float ___m_fontSizeMin_86;
	// System.Single TMPro.TMP_Text::m_fontSizeMax
	float ___m_fontSizeMax_87;
	// TMPro.FontStyles TMPro.TMP_Text::m_fontStyle
	int32_t ___m_fontStyle_88;
	// TMPro.FontStyles TMPro.TMP_Text::m_FontStyleInternal
	int32_t ___m_FontStyleInternal_89;
	// TMPro.TMP_FontStyleStack TMPro.TMP_Text::m_fontStyleStack
	TMP_FontStyleStack_t52885F172FADBC21346C835B5302167BDA8020DC ___m_fontStyleStack_90;
	// System.Boolean TMPro.TMP_Text::m_isUsingBold
	bool ___m_isUsingBold_91;
	// TMPro.HorizontalAlignmentOptions TMPro.TMP_Text::m_HorizontalAlignment
	int32_t ___m_HorizontalAlignment_92;
	// TMPro.VerticalAlignmentOptions TMPro.TMP_Text::m_VerticalAlignment
	int32_t ___m_VerticalAlignment_93;
	// TMPro.TextAlignmentOptions TMPro.TMP_Text::m_textAlignment
	int32_t ___m_textAlignment_94;
	// TMPro.HorizontalAlignmentOptions TMPro.TMP_Text::m_lineJustification
	int32_t ___m_lineJustification_95;
	// TMPro.TMP_TextProcessingStack`1<TMPro.HorizontalAlignmentOptions> TMPro.TMP_Text::m_lineJustificationStack
	TMP_TextProcessingStack_1_t243EA1B5D7FD2295D6533B953F0BBE8F52EFB8A0 ___m_lineJustificationStack_96;
	// UnityEngine.Vector3[] TMPro.TMP_Text::m_textContainerLocalCorners
	Vector3U5BU5D_tFF1859CCE176131B909E2044F76443064254679C* ___m_textContainerLocalCorners_97;
	// System.Single TMPro.TMP_Text::m_characterSpacing
	float ___m_characterSpacing_98;
	// System.Single TMPro.TMP_Text::m_cSpacing
	float ___m_cSpacing_99;
	// System.Single TMPro.TMP_Text::m_monoSpacing
	float ___m_monoSpacing_100;
	// System.Single TMPro.TMP_Text::m_wordSpacing
	float ___m_wordSpacing_101;
	// System.Single TMPro.TMP_Text::m_lineSpacing
	float ___m_lineSpacing_102;
	// System.Single TMPro.TMP_Text::m_lineSpacingDelta
	float ___m_lineSpacingDelta_103;
	// System.Single TMPro.TMP_Text::m_lineHeight
	float ___m_lineHeight_104;
	// System.Boolean TMPro.TMP_Text::m_IsDrivenLineSpacing
	bool ___m_IsDrivenLineSpacing_105;
	// System.Single TMPro.TMP_Text::m_lineSpacingMax
	float ___m_lineSpacingMax_106;
	// System.Single TMPro.TMP_Text::m_paragraphSpacing
	float ___m_paragraphSpacing_107;
	// System.Single TMPro.TMP_Text::m_charWidthMaxAdj
	float ___m_charWidthMaxAdj_108;
	// System.Single TMPro.TMP_Text::m_charWidthAdjDelta
	float ___m_charWidthAdjDelta_109;
	// System.Boolean TMPro.TMP_Text::m_enableWordWrapping
	bool ___m_enableWordWrapping_110;
	// System.Boolean TMPro.TMP_Text::m_isCharacterWrappingEnabled
	bool ___m_isCharacterWrappingEnabled_111;
	// System.Boolean TMPro.TMP_Text::m_isNonBreakingSpace
	bool ___m_isNonBreakingSpace_112;
	// System.Boolean TMPro.TMP_Text::m_isIgnoringAlignment
	bool ___m_isIgnoringAlignment_113;
	// System.Single TMPro.TMP_Text::m_wordWrappingRatios
	float ___m_wordWrappingRatios_114;
	// TMPro.TextOverflowModes TMPro.TMP_Text::m_overflowMode
	int32_t ___m_overflowMode_115;
	// System.Int32 TMPro.TMP_Text::m_firstOverflowCharacterIndex
	int32_t ___m_firstOverflowCharacterIndex_116;
	// TMPro.TMP_Text TMPro.TMP_Text::m_linkedTextComponent
	TMP_Text_tE8D677872D43AD4B2AAF0D6101692A17D0B251A9* ___m_linkedTextComponent_117;
	// TMPro.TMP_Text TMPro.TMP_Text::parentLinkedComponent
	TMP_Text_tE8D677872D43AD4B2AAF0D6101692A17D0B251A9* ___parentLinkedComponent_118;
	// System.Boolean TMPro.TMP_Text::m_isTextTruncated
	bool ___m_isTextTruncated_119;
	// System.Boolean TMPro.TMP_Text::m_enableKerning
	bool ___m_enableKerning_120;
	// System.Single TMPro.TMP_Text::m_GlyphHorizontalAdvanceAdjustment
	float ___m_GlyphHorizontalAdvanceAdjustment_121;
	// System.Boolean TMPro.TMP_Text::m_enableExtraPadding
	bool ___m_enableExtraPadding_122;
	// System.Boolean TMPro.TMP_Text::checkPaddingRequired
	bool ___checkPaddingRequired_123;
	// System.Boolean TMPro.TMP_Text::m_isRichText
	bool ___m_isRichText_124;
	// System.Boolean TMPro.TMP_Text::m_parseCtrlCharacters
	bool ___m_parseCtrlCharacters_125;
	// System.Boolean TMPro.TMP_Text::m_isOverlay
	bool ___m_isOverlay_126;
	// System.Boolean TMPro.TMP_Text::m_isOrthographic
	bool ___m_isOrthographic_127;
	// System.Boolean TMPro.TMP_Text::m_isCullingEnabled
	bool ___m_isCullingEnabled_128;
	// System.Boolean TMPro.TMP_Text::m_isMaskingEnabled
	bool ___m_isMaskingEnabled_129;
	// System.Boolean TMPro.TMP_Text::isMaskUpdateRequired
	bool ___isMaskUpdateRequired_130;
	// System.Boolean TMPro.TMP_Text::m_ignoreCulling
	bool ___m_ignoreCulling_131;
	// TMPro.TextureMappingOptions TMPro.TMP_Text::m_horizontalMapping
	int32_t ___m_horizontalMapping_132;
	// TMPro.TextureMappingOptions TMPro.TMP_Text::m_verticalMapping
	int32_t ___m_verticalMapping_133;
	// System.Single TMPro.TMP_Text::m_uvLineOffset
	float ___m_uvLineOffset_134;
	// TMPro.TextRenderFlags TMPro.TMP_Text::m_renderMode
	int32_t ___m_renderMode_135;
	// TMPro.VertexSortingOrder TMPro.TMP_Text::m_geometrySortingOrder
	int32_t ___m_geometrySortingOrder_136;
	// System.Boolean TMPro.TMP_Text::m_IsTextObjectScaleStatic
	bool ___m_IsTextObjectScaleStatic_137;
	// System.Boolean TMPro.TMP_Text::m_VertexBufferAutoSizeReduction
	bool ___m_VertexBufferAutoSizeReduction_138;
	// System.Int32 TMPro.TMP_Text::m_firstVisibleCharacter
	int32_t ___m_firstVisibleCharacter_139;
	// System.Int32 TMPro.TMP_Text::m_maxVisibleCharacters
	int32_t ___m_maxVisibleCharacters_140;
	// System.Int32 TMPro.TMP_Text::m_maxVisibleWords
	int32_t ___m_maxVisibleWords_141;
	// System.Int32 TMPro.TMP_Text::m_maxVisibleLines
	int32_t ___m_maxVisibleLines_142;
	// System.Boolean TMPro.TMP_Text::m_useMaxVisibleDescender
	bool ___m_useMaxVisibleDescender_143;
	// System.Int32 TMPro.TMP_Text::m_pageToDisplay
	int32_t ___m_pageToDisplay_144;
	// System.Boolean TMPro.TMP_Text::m_isNewPage
	bool ___m_isNewPage_145;
	// UnityEngine.Vector4 TMPro.TMP_Text::m_margin
	Vector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3 ___m_margin_146;
	// System.Single TMPro.TMP_Text::m_marginLeft
	float ___m_marginLeft_147;
	// System.Single TMPro.TMP_Text::m_marginRight
	float ___m_marginRight_148;
	// System.Single TMPro.TMP_Text::m_marginWidth
	float ___m_marginWidth_149;
	// System.Single TMPro.TMP_Text::m_marginHeight
	float ___m_marginHeight_150;
	// System.Single TMPro.TMP_Text::m_width
	float ___m_width_151;
	// TMPro.TMP_TextInfo TMPro.TMP_Text::m_textInfo
	TMP_TextInfo_t09A8E906329422C3F0C059876801DD695B8D524D* ___m_textInfo_152;
	// System.Boolean TMPro.TMP_Text::m_havePropertiesChanged
	bool ___m_havePropertiesChanged_153;
	// System.Boolean TMPro.TMP_Text::m_isUsingLegacyAnimationComponent
	bool ___m_isUsingLegacyAnimationComponent_154;
	// UnityEngine.Transform TMPro.TMP_Text::m_transform
	Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* ___m_transform_155;
	// UnityEngine.RectTransform TMPro.TMP_Text::m_rectTransform
	RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* ___m_rectTransform_156;
	// UnityEngine.Vector2 TMPro.TMP_Text::m_PreviousRectTransformSize
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___m_PreviousRectTransformSize_157;
	// UnityEngine.Vector2 TMPro.TMP_Text::m_PreviousPivotPosition
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___m_PreviousPivotPosition_158;
	// System.Boolean TMPro.TMP_Text::<autoSizeTextContainer>k__BackingField
	bool ___U3CautoSizeTextContainerU3Ek__BackingField_159;
	// System.Boolean TMPro.TMP_Text::m_autoSizeTextContainer
	bool ___m_autoSizeTextContainer_160;
	// UnityEngine.Mesh TMPro.TMP_Text::m_mesh
	Mesh_t6D9C539763A09BC2B12AEAEF36F6DFFC98AE63D4* ___m_mesh_161;
	// System.Boolean TMPro.TMP_Text::m_isVolumetricText
	bool ___m_isVolumetricText_162;
	// System.Action`1<TMPro.TMP_TextInfo> TMPro.TMP_Text::OnPreRenderText
	Action_1_tB93AB717F9D419A1BEC832FF76E74EAA32184CC1* ___OnPreRenderText_165;
	// TMPro.TMP_SpriteAnimator TMPro.TMP_Text::m_spriteAnimator
	TMP_SpriteAnimator_t2E0F016A61CA343E3222FF51E7CF0E53F9F256E4* ___m_spriteAnimator_166;
	// System.Single TMPro.TMP_Text::m_flexibleHeight
	float ___m_flexibleHeight_167;
	// System.Single TMPro.TMP_Text::m_flexibleWidth
	float ___m_flexibleWidth_168;
	// System.Single TMPro.TMP_Text::m_minWidth
	float ___m_minWidth_169;
	// System.Single TMPro.TMP_Text::m_minHeight
	float ___m_minHeight_170;
	// System.Single TMPro.TMP_Text::m_maxWidth
	float ___m_maxWidth_171;
	// System.Single TMPro.TMP_Text::m_maxHeight
	float ___m_maxHeight_172;
	// UnityEngine.UI.LayoutElement TMPro.TMP_Text::m_LayoutElement
	LayoutElement_tB1F24CC11AF4AA87015C8D8EE06D22349C5BF40A* ___m_LayoutElement_173;
	// System.Single TMPro.TMP_Text::m_preferredWidth
	float ___m_preferredWidth_174;
	// System.Single TMPro.TMP_Text::m_renderedWidth
	float ___m_renderedWidth_175;
	// System.Boolean TMPro.TMP_Text::m_isPreferredWidthDirty
	bool ___m_isPreferredWidthDirty_176;
	// System.Single TMPro.TMP_Text::m_preferredHeight
	float ___m_preferredHeight_177;
	// System.Single TMPro.TMP_Text::m_renderedHeight
	float ___m_renderedHeight_178;
	// System.Boolean TMPro.TMP_Text::m_isPreferredHeightDirty
	bool ___m_isPreferredHeightDirty_179;
	// System.Boolean TMPro.TMP_Text::m_isCalculatingPreferredValues
	bool ___m_isCalculatingPreferredValues_180;
	// System.Int32 TMPro.TMP_Text::m_layoutPriority
	int32_t ___m_layoutPriority_181;
	// System.Boolean TMPro.TMP_Text::m_isLayoutDirty
	bool ___m_isLayoutDirty_182;
	// System.Boolean TMPro.TMP_Text::m_isAwake
	bool ___m_isAwake_183;
	// System.Boolean TMPro.TMP_Text::m_isWaitingOnResourceLoad
	bool ___m_isWaitingOnResourceLoad_184;
	// TMPro.TMP_Text/TextInputSources TMPro.TMP_Text::m_inputSource
	int32_t ___m_inputSource_185;
	// System.Single TMPro.TMP_Text::m_fontScaleMultiplier
	float ___m_fontScaleMultiplier_186;
	// System.Single TMPro.TMP_Text::tag_LineIndent
	float ___tag_LineIndent_190;
	// System.Single TMPro.TMP_Text::tag_Indent
	float ___tag_Indent_191;
	// TMPro.TMP_TextProcessingStack`1<System.Single> TMPro.TMP_Text::m_indentStack
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___m_indentStack_192;
	// System.Boolean TMPro.TMP_Text::tag_NoParsing
	bool ___tag_NoParsing_193;
	// System.Boolean TMPro.TMP_Text::m_isParsingText
	bool ___m_isParsingText_194;
	// UnityEngine.Matrix4x4 TMPro.TMP_Text::m_FXMatrix
	Matrix4x4_tDB70CF134A14BA38190C59AA700BCE10E2AED3E6 ___m_FXMatrix_195;
	// System.Boolean TMPro.TMP_Text::m_isFXMatrixSet
	bool ___m_isFXMatrixSet_196;
	// TMPro.TMP_Text/UnicodeChar[] TMPro.TMP_Text::m_TextProcessingArray
	UnicodeCharU5BU5D_t67F27D09F8EB28D2C42DFF16FE60054F157012F5* ___m_TextProcessingArray_197;
	// System.Int32 TMPro.TMP_Text::m_InternalTextProcessingArraySize
	int32_t ___m_InternalTextProcessingArraySize_198;
	// TMPro.TMP_CharacterInfo[] TMPro.TMP_Text::m_internalCharacterInfo
	TMP_CharacterInfoU5BU5D_t297D56FCF66DAA99D8FEA7C30F9F3926902C5B99* ___m_internalCharacterInfo_199;
	// System.Int32 TMPro.TMP_Text::m_totalCharacterCount
	int32_t ___m_totalCharacterCount_200;
	// System.Int32 TMPro.TMP_Text::m_characterCount
	int32_t ___m_characterCount_207;
	// System.Int32 TMPro.TMP_Text::m_firstCharacterOfLine
	int32_t ___m_firstCharacterOfLine_208;
	// System.Int32 TMPro.TMP_Text::m_firstVisibleCharacterOfLine
	int32_t ___m_firstVisibleCharacterOfLine_209;
	// System.Int32 TMPro.TMP_Text::m_lastCharacterOfLine
	int32_t ___m_lastCharacterOfLine_210;
	// System.Int32 TMPro.TMP_Text::m_lastVisibleCharacterOfLine
	int32_t ___m_lastVisibleCharacterOfLine_211;
	// System.Int32 TMPro.TMP_Text::m_lineNumber
	int32_t ___m_lineNumber_212;
	// System.Int32 TMPro.TMP_Text::m_lineVisibleCharacterCount
	int32_t ___m_lineVisibleCharacterCount_213;
	// System.Int32 TMPro.TMP_Text::m_pageNumber
	int32_t ___m_pageNumber_214;
	// System.Single TMPro.TMP_Text::m_PageAscender
	float ___m_PageAscender_215;
	// System.Single TMPro.TMP_Text::m_maxTextAscender
	float ___m_maxTextAscender_216;
	// System.Single TMPro.TMP_Text::m_maxCapHeight
	float ___m_maxCapHeight_217;
	// System.Single TMPro.TMP_Text::m_ElementAscender
	float ___m_ElementAscender_218;
	// System.Single TMPro.TMP_Text::m_ElementDescender
	float ___m_ElementDescender_219;
	// System.Single TMPro.TMP_Text::m_maxLineAscender
	float ___m_maxLineAscender_220;
	// System.Single TMPro.TMP_Text::m_maxLineDescender
	float ___m_maxLineDescender_221;
	// System.Single TMPro.TMP_Text::m_startOfLineAscender
	float ___m_startOfLineAscender_222;
	// System.Single TMPro.TMP_Text::m_startOfLineDescender
	float ___m_startOfLineDescender_223;
	// System.Single TMPro.TMP_Text::m_lineOffset
	float ___m_lineOffset_224;
	// TMPro.Extents TMPro.TMP_Text::m_meshExtents
	Extents_tA2D2F95811D0A18CB7AC3570D2D8F8CD3AF4C4A8 ___m_meshExtents_225;
	// UnityEngine.Color32 TMPro.TMP_Text::m_htmlColor
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___m_htmlColor_226;
	// TMPro.TMP_TextProcessingStack`1<UnityEngine.Color32> TMPro.TMP_Text::m_colorStack
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___m_colorStack_227;
	// TMPro.TMP_TextProcessingStack`1<UnityEngine.Color32> TMPro.TMP_Text::m_underlineColorStack
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___m_underlineColorStack_228;
	// TMPro.TMP_TextProcessingStack`1<UnityEngine.Color32> TMPro.TMP_Text::m_strikethroughColorStack
	TMP_TextProcessingStack_1_tF2CD5BE59E5EB22EA9E3EE3043A004EA918C4BB3 ___m_strikethroughColorStack_229;
	// TMPro.TMP_TextProcessingStack`1<TMPro.HighlightState> TMPro.TMP_Text::m_HighlightStateStack
	TMP_TextProcessingStack_1_t57AECDCC936A7FF1D6CF66CA11560B28A675648D ___m_HighlightStateStack_230;
	// TMPro.TMP_ColorGradient TMPro.TMP_Text::m_colorGradientPreset
	TMP_ColorGradient_t17B51752B4E9499A1FF7D875DCEC1D15A0F4AEBB* ___m_colorGradientPreset_231;
	// TMPro.TMP_TextProcessingStack`1<TMPro.TMP_ColorGradient> TMPro.TMP_Text::m_colorGradientStack
	TMP_TextProcessingStack_1_tC8FAEB17246D3B171EFD11165A5761AE39B40D0C ___m_colorGradientStack_232;
	// System.Boolean TMPro.TMP_Text::m_colorGradientPresetIsTinted
	bool ___m_colorGradientPresetIsTinted_233;
	// System.Single TMPro.TMP_Text::m_tabSpacing
	float ___m_tabSpacing_234;
	// System.Single TMPro.TMP_Text::m_spacing
	float ___m_spacing_235;
	// TMPro.TMP_TextProcessingStack`1<System.Int32>[] TMPro.TMP_Text::m_TextStyleStacks
	TMP_TextProcessingStack_1U5BU5D_t08293E0BB072311BB96170F351D1083BCA97B9B2* ___m_TextStyleStacks_236;
	// System.Int32 TMPro.TMP_Text::m_TextStyleStackDepth
	int32_t ___m_TextStyleStackDepth_237;
	// TMPro.TMP_TextProcessingStack`1<System.Int32> TMPro.TMP_Text::m_ItalicAngleStack
	TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C ___m_ItalicAngleStack_238;
	// System.Int32 TMPro.TMP_Text::m_ItalicAngle
	int32_t ___m_ItalicAngle_239;
	// TMPro.TMP_TextProcessingStack`1<System.Int32> TMPro.TMP_Text::m_actionStack
	TMP_TextProcessingStack_1_tFBA719426D68CE1F2B5849D97AF5E5D65846290C ___m_actionStack_240;
	// System.Single TMPro.TMP_Text::m_padding
	float ___m_padding_241;
	// System.Single TMPro.TMP_Text::m_baselineOffset
	float ___m_baselineOffset_242;
	// TMPro.TMP_TextProcessingStack`1<System.Single> TMPro.TMP_Text::m_baselineOffsetStack
	TMP_TextProcessingStack_1_t138EC06BE7F101AA0A3C8D2DC951E55AACE085E9 ___m_baselineOffsetStack_243;
	// System.Single TMPro.TMP_Text::m_xAdvance
	float ___m_xAdvance_244;
	// TMPro.TMP_TextElementType TMPro.TMP_Text::m_textElementType
	int32_t ___m_textElementType_245;
	// TMPro.TMP_TextElement TMPro.TMP_Text::m_cached_TextElement
	TMP_TextElement_t262A55214F712D4274485ABE5676E5254B84D0A5* ___m_cached_TextElement_246;
	// TMPro.TMP_Text/SpecialCharacter TMPro.TMP_Text::m_Ellipsis
	SpecialCharacter_t6C1DBE8C490706D1620899BAB7F0B8091AD26777 ___m_Ellipsis_247;
	// TMPro.TMP_Text/SpecialCharacter TMPro.TMP_Text::m_Underline
	SpecialCharacter_t6C1DBE8C490706D1620899BAB7F0B8091AD26777 ___m_Underline_248;
	// TMPro.TMP_SpriteAsset TMPro.TMP_Text::m_defaultSpriteAsset
	TMP_SpriteAsset_t81F779E6F705CE190DC0D1F93A954CB8B1774B39* ___m_defaultSpriteAsset_249;
	// TMPro.TMP_SpriteAsset TMPro.TMP_Text::m_currentSpriteAsset
	TMP_SpriteAsset_t81F779E6F705CE190DC0D1F93A954CB8B1774B39* ___m_currentSpriteAsset_250;
	// System.Int32 TMPro.TMP_Text::m_spriteCount
	int32_t ___m_spriteCount_251;
	// System.Int32 TMPro.TMP_Text::m_spriteIndex
	int32_t ___m_spriteIndex_252;
	// System.Int32 TMPro.TMP_Text::m_spriteAnimationID
	int32_t ___m_spriteAnimationID_253;
	// System.Boolean TMPro.TMP_Text::m_ignoreActiveState
	bool ___m_ignoreActiveState_256;
	// TMPro.TMP_Text/TextBackingContainer TMPro.TMP_Text::m_TextBackingArray
	TextBackingContainer_t33D1CE628E7B26C45EDAC1D87BEF2DD22A5C6361 ___m_TextBackingArray_257;
	// System.Decimal[] TMPro.TMP_Text::k_Power
	DecimalU5BU5D_t93BA0C88FA80728F73B792EE1A5199D0C060B615* ___k_Power_258;
};

struct TMP_Text_tE8D677872D43AD4B2AAF0D6101692A17D0B251A9_StaticFields
{
	// TMPro.MaterialReference[] TMPro.TMP_Text::m_materialReferences
	MaterialReferenceU5BU5D_t7491D335AB3E3E13CE9C0F5E931F396F6A02E1F2* ___m_materialReferences_45;
	// System.Collections.Generic.Dictionary`2<System.Int32,System.Int32> TMPro.TMP_Text::m_materialReferenceIndexLookup
	Dictionary_2_tABE19B9C5C52F1DE14F0D3287B2696E7D7419180* ___m_materialReferenceIndexLookup_46;
	// TMPro.TMP_TextProcessingStack`1<TMPro.MaterialReference> TMPro.TMP_Text::m_materialReferenceStack
	TMP_TextProcessingStack_1_tB03E08F69415B281A5A81138F09E49EE58402DF9 ___m_materialReferenceStack_47;
	// UnityEngine.Color32 TMPro.TMP_Text::s_colorWhite
	Color32_t73C5004937BF5BB8AD55323D51AAA40A898EF48B ___s_colorWhite_55;
	// System.Func`3<System.Int32,System.String,TMPro.TMP_FontAsset> TMPro.TMP_Text::OnFontAssetRequest
	Func_3_tC721DF8CDD07ED66A4833A19A2ED2302608C906C* ___OnFontAssetRequest_163;
	// System.Func`3<System.Int32,System.String,TMPro.TMP_SpriteAsset> TMPro.TMP_Text::OnSpriteAssetRequest
	Func_3_t6F6D9932638EA1A5A45303C6626C818C25D164E5* ___OnSpriteAssetRequest_164;
	// System.Char[] TMPro.TMP_Text::m_htmlTag
	CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB* ___m_htmlTag_187;
	// TMPro.RichTextTagAttribute[] TMPro.TMP_Text::m_xmlAttribute
	RichTextTagAttributeU5BU5D_t5816316EFD8F59DBC30B9F88E15828C564E47B6D* ___m_xmlAttribute_188;
	// System.Single[] TMPro.TMP_Text::m_attributeParameterValues
	SingleU5BU5D_t89DEFE97BCEDB5857010E79ECE0F52CF6E93B87C* ___m_attributeParameterValues_189;
	// TMPro.WordWrapState TMPro.TMP_Text::m_SavedWordWrapState
	WordWrapState_t80F67D8CAA9B1A0A3D5266521E23A9F3100EDD0A ___m_SavedWordWrapState_201;
	// TMPro.WordWrapState TMPro.TMP_Text::m_SavedLineState
	WordWrapState_t80F67D8CAA9B1A0A3D5266521E23A9F3100EDD0A ___m_SavedLineState_202;
	// TMPro.WordWrapState TMPro.TMP_Text::m_SavedEllipsisState
	WordWrapState_t80F67D8CAA9B1A0A3D5266521E23A9F3100EDD0A ___m_SavedEllipsisState_203;
	// TMPro.WordWrapState TMPro.TMP_Text::m_SavedLastValidState
	WordWrapState_t80F67D8CAA9B1A0A3D5266521E23A9F3100EDD0A ___m_SavedLastValidState_204;
	// TMPro.WordWrapState TMPro.TMP_Text::m_SavedSoftLineBreakState
	WordWrapState_t80F67D8CAA9B1A0A3D5266521E23A9F3100EDD0A ___m_SavedSoftLineBreakState_205;
	// TMPro.TMP_TextProcessingStack`1<TMPro.WordWrapState> TMPro.TMP_Text::m_EllipsisInsertionCandidateStack
	TMP_TextProcessingStack_1_t2DDA00FFC64AF6E3AFD475AB2086D16C34787E0F ___m_EllipsisInsertionCandidateStack_206;
	// Unity.Profiling.ProfilerMarker TMPro.TMP_Text::k_ParseTextMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_ParseTextMarker_254;
	// Unity.Profiling.ProfilerMarker TMPro.TMP_Text::k_InsertNewLineMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_InsertNewLineMarker_255;
	// UnityEngine.Vector2 TMPro.TMP_Text::k_LargePositiveVector2
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___k_LargePositiveVector2_259;
	// UnityEngine.Vector2 TMPro.TMP_Text::k_LargeNegativeVector2
	Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___k_LargeNegativeVector2_260;
	// System.Single TMPro.TMP_Text::k_LargePositiveFloat
	float ___k_LargePositiveFloat_261;
	// System.Single TMPro.TMP_Text::k_LargeNegativeFloat
	float ___k_LargeNegativeFloat_262;
	// System.Int32 TMPro.TMP_Text::k_LargePositiveInt
	int32_t ___k_LargePositiveInt_263;
	// System.Int32 TMPro.TMP_Text::k_LargeNegativeInt
	int32_t ___k_LargeNegativeInt_264;
};

// TMPro.TextMeshProUGUI
struct TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957  : public TMP_Text_tE8D677872D43AD4B2AAF0D6101692A17D0B251A9
{
	// System.Boolean TMPro.TextMeshProUGUI::m_hasFontAssetChanged
	bool ___m_hasFontAssetChanged_265;
	// TMPro.TMP_SubMeshUI[] TMPro.TextMeshProUGUI::m_subTextObjects
	TMP_SubMeshUIU5BU5D_tC77B263183A59A75345C26152457207EAC3BBF29* ___m_subTextObjects_266;
	// System.Single TMPro.TextMeshProUGUI::m_previousLossyScaleY
	float ___m_previousLossyScaleY_267;
	// UnityEngine.Vector3[] TMPro.TextMeshProUGUI::m_RectTransformCorners
	Vector3U5BU5D_tFF1859CCE176131B909E2044F76443064254679C* ___m_RectTransformCorners_268;
	// UnityEngine.CanvasRenderer TMPro.TextMeshProUGUI::m_canvasRenderer
	CanvasRenderer_tAB9A55A976C4E3B2B37D0CE5616E5685A8B43860* ___m_canvasRenderer_269;
	// UnityEngine.Canvas TMPro.TextMeshProUGUI::m_canvas
	Canvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26* ___m_canvas_270;
	// System.Single TMPro.TextMeshProUGUI::m_CanvasScaleFactor
	float ___m_CanvasScaleFactor_271;
	// System.Boolean TMPro.TextMeshProUGUI::m_isFirstAllocation
	bool ___m_isFirstAllocation_272;
	// System.Int32 TMPro.TextMeshProUGUI::m_max_characters
	int32_t ___m_max_characters_273;
	// UnityEngine.Material TMPro.TextMeshProUGUI::m_baseMaterial
	Material_t18053F08F347D0DCA5E1140EC7EC4533DD8A14E3* ___m_baseMaterial_274;
	// System.Boolean TMPro.TextMeshProUGUI::m_isScrollRegionSet
	bool ___m_isScrollRegionSet_275;
	// UnityEngine.Vector4 TMPro.TextMeshProUGUI::m_maskOffset
	Vector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3 ___m_maskOffset_276;
	// UnityEngine.Matrix4x4 TMPro.TextMeshProUGUI::m_EnvMapMatrix
	Matrix4x4_tDB70CF134A14BA38190C59AA700BCE10E2AED3E6 ___m_EnvMapMatrix_277;
	// System.Boolean TMPro.TextMeshProUGUI::m_isRegisteredForEvents
	bool ___m_isRegisteredForEvents_278;
	// System.Boolean TMPro.TextMeshProUGUI::m_isRebuildingLayout
	bool ___m_isRebuildingLayout_299;
	// UnityEngine.Coroutine TMPro.TextMeshProUGUI::m_DelayedGraphicRebuild
	Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* ___m_DelayedGraphicRebuild_300;
	// UnityEngine.Coroutine TMPro.TextMeshProUGUI::m_DelayedMaterialRebuild
	Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* ___m_DelayedMaterialRebuild_301;
	// UnityEngine.Rect TMPro.TextMeshProUGUI::m_ClipRect
	Rect_tA04E0F8A1830E767F40FB27ECD8D309303571F0D ___m_ClipRect_302;
	// System.Boolean TMPro.TextMeshProUGUI::m_ValidRect
	bool ___m_ValidRect_303;
	// System.Action`1<TMPro.TMP_TextInfo> TMPro.TextMeshProUGUI::OnPreRenderText
	Action_1_tB93AB717F9D419A1BEC832FF76E74EAA32184CC1* ___OnPreRenderText_304;
};

struct TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957_StaticFields
{
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_GenerateTextMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_GenerateTextMarker_279;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_SetArraySizesMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_SetArraySizesMarker_280;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_GenerateTextPhaseIMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_GenerateTextPhaseIMarker_281;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_ParseMarkupTextMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_ParseMarkupTextMarker_282;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_CharacterLookupMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_CharacterLookupMarker_283;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_HandleGPOSFeaturesMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_HandleGPOSFeaturesMarker_284;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_CalculateVerticesPositionMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_CalculateVerticesPositionMarker_285;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_ComputeTextMetricsMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_ComputeTextMetricsMarker_286;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_HandleVisibleCharacterMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_HandleVisibleCharacterMarker_287;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_HandleWhiteSpacesMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_HandleWhiteSpacesMarker_288;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_HandleHorizontalLineBreakingMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_HandleHorizontalLineBreakingMarker_289;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_HandleVerticalLineBreakingMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_HandleVerticalLineBreakingMarker_290;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_SaveGlyphVertexDataMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_SaveGlyphVertexDataMarker_291;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_ComputeCharacterAdvanceMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_ComputeCharacterAdvanceMarker_292;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_HandleCarriageReturnMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_HandleCarriageReturnMarker_293;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_HandleLineTerminationMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_HandleLineTerminationMarker_294;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_SavePageInfoMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_SavePageInfoMarker_295;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_SaveProcessingStatesMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_SaveProcessingStatesMarker_296;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_GenerateTextPhaseIIMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_GenerateTextPhaseIIMarker_297;
	// Unity.Profiling.ProfilerMarker TMPro.TextMeshProUGUI::k_GenerateTextPhaseIIIMarker
	ProfilerMarker_tA256E18DA86EDBC5528CE066FC91C96EE86501AD ___k_GenerateTextPhaseIIIMarker_298;
};
#ifdef __clang__
#pragma clang diagnostic pop
#endif
// Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType[]
struct __Il2CppFullySharedGenericTypeU5BU5D_tCAB6D060972DD49223A834B7EEFEB9FE2D003BEC  : public RuntimeArray
{
	ALIGN_FIELD (8) uint8_t m_Items[1];

	inline uint8_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + il2cpp_array_calc_byte_offset(this, index);
	}
	inline uint8_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + il2cpp_array_calc_byte_offset(this, index);
	}
};


// TValue System.Collections.Generic.Dictionary`2<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType,Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>::get_Item(TKey)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2_get_Item_m2E96908E9716367701CD737FA54C884EB2A8C3EA_gshared (Dictionary_2_t5C32AF17A5801FB3109E5B0E622BA8402A04E08E* __this, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType*/Il2CppFullySharedGenericAny ___key0, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny* il2cppRetVal, const RuntimeMethod* method);
// System.Collections.Generic.List`1<TSource> System.Linq.Enumerable::ToList<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>(System.Collections.Generic.IEnumerable`1<TSource>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A* Enumerable_ToList_TisIl2CppFullySharedGenericAny_mAD7ADD5EE3D6E0A30271EDC31500BCE6301A256D_gshared (RuntimeObject* ___source0, const RuntimeMethod* method);
// System.Collections.Generic.List`1/Enumerator<T> System.Collections.Generic.List`1<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>::GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void List_1_GetEnumerator_m8B2A92ACD4FBA5FBDC3F6F4F5C23A0DDF491DA61_gshared (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A* __this, Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF* il2cppRetVal, const RuntimeMethod* method);
// T System.Collections.Generic.List`1/Enumerator<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>::get_Current()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Enumerator_get_Current_m8B42D4B2DE853B9D11B997120CD0228D4780E394_gshared_inline (Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF* __this, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny* il2cppRetVal, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.List`1<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>::Remove(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool List_1_Remove_m9BCE8CEF94E6F2BF8624D65214FF4F3CA686D60C_gshared (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A* __this, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType*/Il2CppFullySharedGenericAny ___item0, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.List`1/Enumerator<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_m8D8E5E878AF0A88A535AB1AB5BA4F23E151A678A_gshared (Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF* __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1/Enumerator<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Enumerator_Dispose_mFE1EBE6F6425283FEAEAE7C79D02CDE4F9D367E8_gshared (Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF* __this, const RuntimeMethod* method);
// T System.Collections.Generic.List`1<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void List_1_get_Item_m6E4BA37C1FB558E4A62AE4324212E45D09C5C937_gshared (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A* __this, int32_t ___index0, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny* il2cppRetVal, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.List`1<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>::get_Count()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t List_1_get_Count_mD2ED26ACAF3BAF386FFEA83893BA51DB9FD8BA30_gshared_inline (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A* __this, const RuntimeMethod* method);
// T UnityEngine.Component::GetComponent<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Component_GetComponent_TisIl2CppFullySharedGenericAny_m47CBDD147982125387F078ABBFDAAB92D397A6C2_gshared (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny* il2cppRetVal, const RuntimeMethod* method);
// T UnityEngine.Object::FindObjectOfType<System.Object>()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Object_FindObjectOfType_TisRuntimeObject_m9990A7304DF02BA1ED160587D1C2F6DAE89BB343_gshared (const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void List_1__ctor_m0AFBAEA7EC427E32CC9CA267B1930DC5DF67A374_gshared (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A* __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>::Add(T)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void List_1_Add_mD4F3498FBD3BDD3F03CBCFB38041CBAC9C28CAFC_gshared_inline (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A* __this, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType*/Il2CppFullySharedGenericAny ___item0, const RuntimeMethod* method);
// System.Void UnityEngine.Events.UnityAction`2<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType,Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnityAction_2__ctor_m17203366119014F4963976DF6B8E83DE49274252_gshared (UnityAction_2_t742C43FA6EAABE0458C753DFE15FDDFAE01EA73F* __this, RuntimeObject* ___object0, intptr_t ___method1, const RuntimeMethod* method);
// T UnityEngine.Component::GetComponentInChildren<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Component_GetComponentInChildren_TisIl2CppFullySharedGenericAny_m6C912B287F81A629FB1D697E7CEB80D3B940295F_gshared (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny* il2cppRetVal, const RuntimeMethod* method);
// T UnityEngine.GameObject::AddComponent<System.Object>()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* GameObject_AddComponent_TisRuntimeObject_m69B93700FACCF372F5753371C6E8FB780800B824_gshared (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method);
// T UnityEngine.GameObject::GetComponent<Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType>()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameObject_GetComponent_TisIl2CppFullySharedGenericAny_m1122128E432233EB251AECF734E2B72A42A2C194_gshared (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny* il2cppRetVal, const RuntimeMethod* method);

// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2 (RuntimeObject* __this, const RuntimeMethod* method);
// System.Boolean System.String::op_Equality(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_op_Equality_m0D685A924E5CD78078F248ED1726DA5A9D7D6AC0 (String_t* ___a0, String_t* ___b1, const RuntimeMethod* method);
// TValue System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.List`1<Dice>>::get_Item(TKey)
inline List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* Dictionary_2_get_Item_mBEB955470239D94BCB11C23D263104688CC2595B (Dictionary_2_t71DC4111D9530C502ACEA7A17068D6DC7AF41689* __this, String_t* ___key0, const RuntimeMethod* method)
{
	List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* il2cppRetVal;
	((  void (*) (Dictionary_2_t5C32AF17A5801FB3109E5B0E622BA8402A04E08E*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType*/Il2CppFullySharedGenericAny, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny*, const RuntimeMethod*))Dictionary_2_get_Item_m2E96908E9716367701CD737FA54C884EB2A8C3EA_gshared)((Dictionary_2_t5C32AF17A5801FB3109E5B0E622BA8402A04E08E*)__this, (Il2CppFullySharedGenericAny)___key0, (Il2CppFullySharedGenericAny*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// TValue System.Collections.Generic.Dictionary`2<System.Single,UnityEngine.WaitForSeconds>::get_Item(TKey)
inline WaitForSeconds_tF179DF251655B8DF044952E70A60DF4B358A3DD3* Dictionary_2_get_Item_mF08AF6105F6A747B98757834E9BE7FE975620925 (Dictionary_2_t5826E39B66190EFF2D3EA81361D5FC4BB8D6B212* __this, float ___key0, const RuntimeMethod* method)
{
	WaitForSeconds_tF179DF251655B8DF044952E70A60DF4B358A3DD3* il2cppRetVal;
	((  void (*) (Dictionary_2_t5C32AF17A5801FB3109E5B0E622BA8402A04E08E*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType*/Il2CppFullySharedGenericAny, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny*, const RuntimeMethod*))Dictionary_2_get_Item_m2E96908E9716367701CD737FA54C884EB2A8C3EA_gshared)((Dictionary_2_t5C32AF17A5801FB3109E5B0E622BA8402A04E08E*)__this, (Il2CppFullySharedGenericAny)&___key0, (Il2CppFullySharedGenericAny*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// System.Collections.Generic.List`1<TSource> System.Linq.Enumerable::ToList<Dice>(System.Collections.Generic.IEnumerable`1<TSource>)
inline List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* Enumerable_ToList_TisDice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A_m34AE9657FC95F123345470E6C2B585FC6CD58C19 (RuntimeObject* ___source0, const RuntimeMethod* method)
{
	List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A* il2cppRetVal = ((  List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A* (*) (RuntimeObject*, const RuntimeMethod*))Enumerable_ToList_TisIl2CppFullySharedGenericAny_mAD7ADD5EE3D6E0A30271EDC31500BCE6301A256D_gshared)((RuntimeObject*)___source0, method);
	return (List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789*)il2cppRetVal;
}
// System.Collections.Generic.List`1/Enumerator<T> System.Collections.Generic.List`1<Dice>::GetEnumerator()
inline Enumerator_t4FA7DAAD85BE6888B74F6C499212CB3463719F5D List_1_GetEnumerator_m56D864FDB01A87F36472DFA6F97147CDA63DE0B0 (List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* __this, const RuntimeMethod* method)
{
	Enumerator_t4FA7DAAD85BE6888B74F6C499212CB3463719F5D il2cppRetVal;
	((  void (*) (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*, Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF*, const RuntimeMethod*))List_1_GetEnumerator_m8B2A92ACD4FBA5FBDC3F6F4F5C23A0DDF491DA61_gshared)((List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*)__this, (Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// T System.Collections.Generic.List`1/Enumerator<Dice>::get_Current()
inline Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* Enumerator_get_Current_m789E5F6B4BF5820DBED7D75DB908232A6B38C182_inline (Enumerator_t4FA7DAAD85BE6888B74F6C499212CB3463719F5D* __this, const RuntimeMethod* method)
{
	Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* il2cppRetVal;
	((  void (*) (Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny*, const RuntimeMethod*))Enumerator_get_Current_m8B42D4B2DE853B9D11B997120CD0228D4780E394_gshared_inline)((Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF*)__this, (Il2CppFullySharedGenericAny*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// UnityEngine.GameObject UnityEngine.Component::get_gameObject()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* Component_get_gameObject_m57AEFBB14DB39EC476F740BA000E170355DE691B (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.List`1<UnityEngine.GameObject>::Remove(T)
inline bool List_1_Remove_mCCE85D4D5326536C4B214C73D07030F4CCD18485 (List_1_tB951CE80B58D1BF9650862451D8DAD8C231F207B* __this, GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___item0, const RuntimeMethod* method)
{
	return ((  bool (*) (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType*/Il2CppFullySharedGenericAny, const RuntimeMethod*))List_1_Remove_m9BCE8CEF94E6F2BF8624D65214FF4F3CA686D60C_gshared)((List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*)__this, (Il2CppFullySharedGenericAny)___item0, method);
}
// System.Boolean System.Collections.Generic.List`1<Dice>::Remove(T)
inline bool List_1_Remove_m6ECEFB43E9BD1D878B805AA46E41073EEA8F2C6F (List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* __this, Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* ___item0, const RuntimeMethod* method)
{
	return ((  bool (*) (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType*/Il2CppFullySharedGenericAny, const RuntimeMethod*))List_1_Remove_m9BCE8CEF94E6F2BF8624D65214FF4F3CA686D60C_gshared)((List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*)__this, (Il2CppFullySharedGenericAny)___item0, method);
}
// System.Void UnityEngine.Object::Destroy(UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object_Destroy_mFCDAE6333522488F60597AF019EA90BB1207A5AA (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C* ___obj0, const RuntimeMethod* method);
// System.Void DiceSummoner::SaveDiceValues()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void DiceSummoner_SaveDiceValues_m1694DCB6AEF34961E3CEFD220911269D29007370 (DiceSummoner_tA51E21ADF511DD948DD0B44C50A73CF2F8C29DBC* __this, const RuntimeMethod* method);
// System.Void StatSummoner::SetDebugInformationFor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StatSummoner_SetDebugInformationFor_m9A63F7BD07E872EBD85AF47B9251D0A113B21FB5 (StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539* __this, String_t* ___playerOrEnemy0, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.List`1/Enumerator<Dice>::MoveNext()
inline bool Enumerator_MoveNext_m8E6AC5FCF5864D5C53C3838A777B86D2A9F5FDF7 (Enumerator_t4FA7DAAD85BE6888B74F6C499212CB3463719F5D* __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF*, const RuntimeMethod*))Enumerator_MoveNext_m8D8E5E878AF0A88A535AB1AB5BA4F23E151A678A_gshared)((Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF*)__this, method);
}
// System.Void System.Collections.Generic.List`1/Enumerator<Dice>::Dispose()
inline void Enumerator_Dispose_m183F16152D45183DC6CE1A195F14D3EB174A560D (Enumerator_t4FA7DAAD85BE6888B74F6C499212CB3463719F5D* __this, const RuntimeMethod* method)
{
	((  void (*) (Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF*, const RuntimeMethod*))Enumerator_Dispose_mFE1EBE6F6425283FEAEAE7C79D02CDE4F9D367E8_gshared)((Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF*)__this, method);
}
// T System.Collections.Generic.List`1<Dice>::get_Item(System.Int32)
inline Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6 (List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* __this, int32_t ___index0, const RuntimeMethod* method)
{
	Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* il2cppRetVal;
	((  void (*) (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*, int32_t, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny*, const RuntimeMethod*))List_1_get_Item_m6E4BA37C1FB558E4A62AE4324212E45D09C5C937_gshared)((List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*)__this, ___index0, (Il2CppFullySharedGenericAny*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// UnityEngine.Transform UnityEngine.Component::get_transform()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371 (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method);
// System.Single StatSummoner::OutermostPlayerX(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float StatSummoner_OutermostPlayerX_mBA0D0C3A9EB4A3026DFE44CD7A7E650341980C02 (StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539* __this, String_t* ___statType0, String_t* ___optionalDiceOffsetStatToMultiplyBy1, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.List`1<Dice>::get_Count()
inline int32_t List_1_get_Count_mD2E0CF4E337A946034B00E365D8B3B66445B4044_inline (List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*, const RuntimeMethod*))List_1_get_Count_mD2ED26ACAF3BAF386FFEA83893BA51DB9FD8BA30_gshared_inline)((List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*)__this, method);
}
// UnityEngine.Vector3 UnityEngine.Transform::get_position()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Transform_get_position_m69CD5FA214FDAE7BB701552943674846C220FDE1 (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, const RuntimeMethod* method);
// System.Void UnityEngine.Vector2::.ctor(System.Single,System.Single)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Vector2__ctor_m9525B79969AFFE3254B303A40997A56DEEB6F548_inline (Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7* __this, float ___x0, float ___y1, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector2::op_Implicit(UnityEngine.Vector2)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector2_op_Implicit_mCD214B04BC52AED3C89C3BEF664B6247E5F8954A_inline (Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___v0, const RuntimeMethod* method);
// System.Void UnityEngine.Transform::set_position(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Transform_set_position_mA1A817124BB41B685043DED2A9BA48CDF37C4156 (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___value0, const RuntimeMethod* method);
// System.Single StatSummoner::OutermostEnemyX(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float StatSummoner_OutermostEnemyX_m611089CE18D77FD22CDE74ED71CA364749DF384E (StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539* __this, String_t* ___statType0, const RuntimeMethod* method);
// T UnityEngine.Component::GetComponent<Dice>()
inline Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* Component_GetComponent_TisDice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A_m91F6B03AAEFF32E02B3AC36981E9D444FD235085 (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method)
{
	Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* il2cppRetVal;
	((  void (*) (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny*, const RuntimeMethod*))Component_GetComponent_TisIl2CppFullySharedGenericAny_m47CBDD147982125387F078ABBFDAAB92D397A6C2_gshared)((Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*)__this, (Il2CppFullySharedGenericAny*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// System.Void System.NotSupportedException::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NotSupportedException__ctor_m1398D0CDE19B36AA3DE9392879738C1EA2439CDF (NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A* __this, const RuntimeMethod* method);
// T UnityEngine.Object::FindObjectOfType<Scripts>()
inline Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* Object_FindObjectOfType_TisScripts_tA1F67F81394769A4746B08F203BBD942A423AF5C_mD4A9D0F71D72CAEF1369857214457214E426E335 (const RuntimeMethod* method)
{
	RuntimeObject* il2cppRetVal = ((  RuntimeObject* (*) (const RuntimeMethod*))Object_FindObjectOfType_TisRuntimeObject_m9990A7304DF02BA1ED160587D1C2F6DAE89BB343_gshared)(method);
	return (Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C*)il2cppRetVal;
}
// T UnityEngine.Component::GetComponent<UnityEngine.SpriteRenderer>()
inline SpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B* Component_GetComponent_TisSpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B_m6181F10C09FC1650DAE0EF2308D344A2F170AA45 (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method)
{
	SpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B* il2cppRetVal;
	((  void (*) (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny*, const RuntimeMethod*))Component_GetComponent_TisIl2CppFullySharedGenericAny_m47CBDD147982125387F078ABBFDAAB92D397A6C2_gshared)((Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*)__this, (Il2CppFullySharedGenericAny*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// System.Void UnityEngine.SpriteRenderer::set_sprite(UnityEngine.Sprite)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SpriteRenderer_set_sprite_m7B176E33955108C60CAE21DFC153A0FAC674CB53 (SpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B* __this, Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* ___value0, const RuntimeMethod* method);
// System.Collections.IEnumerator Tutorial::TextAnimation(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Tutorial_TextAnimation_m1720AF20B6881F0494F69C87B96669A98391ED47 (Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* __this, int32_t ___index0, const RuntimeMethod* method);
// UnityEngine.Coroutine UnityEngine.MonoBehaviour::StartCoroutine(System.Collections.IEnumerator)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* MonoBehaviour_StartCoroutine_m4CAFF732AA28CD3BDC5363B44A863575530EC812 (MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71* __this, RuntimeObject* ___routine0, const RuntimeMethod* method);
// System.Void SoundManager::PlayClip(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SoundManager_PlayClip_m45E4AD5F009E3F6679345FF5BAECF1B20F442C58 (SoundManager_tCA2CCAC5CDF1BA10E525C01C8D1D0DFAC9BE3734* __this, String_t* ___clipName0, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.List`1<System.String>::get_Count()
inline int32_t List_1_get_Count_mB63183A9151F4345A9DD444A7CBE0D6E03F77C7C_inline (List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*, const RuntimeMethod*))List_1_get_Count_mD2ED26ACAF3BAF386FFEA83893BA51DB9FD8BA30_gshared_inline)((List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*)__this, method);
}
// System.Void UnityEngine.MonoBehaviour::StopCoroutine(UnityEngine.Coroutine)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MonoBehaviour_StopCoroutine_mB0FC91BE84203BD8E360B3FBAE5B958B4C5ED22A (MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71* __this, Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* ___routine0, const RuntimeMethod* method);
// T System.Collections.Generic.List`1<System.String>::get_Item(System.Int32)
inline String_t* List_1_get_Item_m21AEC50E791371101DC22ABCF96A2E46800811F8 (List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* __this, int32_t ___index0, const RuntimeMethod* method)
{
	String_t* il2cppRetVal;
	((  void (*) (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*, int32_t, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny*, const RuntimeMethod*))List_1_get_Item_m6E4BA37C1FB558E4A62AE4324212E45D09C5C937_gshared)((List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*)__this, ___index0, (Il2CppFullySharedGenericAny*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// System.Void Tutorial::Increment()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tutorial_Increment_mF12EB763F6A8E8375CEF37B46981936AE9DF3276 (Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* __this, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.List`1<UnityEngine.GameObject>::get_Count()
inline int32_t List_1_get_Count_m4C37ED2D928D63B80F55AF434730C2D64EEB9F22_inline (List_1_tB951CE80B58D1BF9650862451D8DAD8C231F207B* __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*, const RuntimeMethod*))List_1_get_Count_mD2ED26ACAF3BAF386FFEA83893BA51DB9FD8BA30_gshared_inline)((List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*)__this, method);
}
// System.Void DiceSummoner::SummonDice(System.Boolean,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void DiceSummoner_SummonDice_mED35D68D43E07F5F6097C300597DEC8D998D1A33 (DiceSummoner_tA51E21ADF511DD948DD0B44C50A73CF2F8C29DBC* __this, bool ___initialSummon0, bool ___newSet1, const RuntimeMethod* method);
// System.Collections.IEnumerator Tutorial::TextAnimation(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Tutorial_TextAnimation_mAC267160B03FE9932679CA6E9E1BF719AFDC32ED (Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* __this, String_t* ___str0, const RuntimeMethod* method);
// System.Void Tutorial/<TextAnimation>d__13::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CTextAnimationU3Ed__13__ctor_mDA59BE84AEE7C584D38F61046A3EE671D28D4C4F (U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02* __this, int32_t ___U3CU3E1__state0, const RuntimeMethod* method);
// System.Void Tutorial/<TextAnimation>d__14::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CTextAnimationU3Ed__14__ctor_m5AD9D83A2EB7D2C21A3F746E47C8FE50FBB7AA5A (U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8* __this, int32_t ___U3CU3E1__state0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<System.String>::.ctor()
inline void List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E (List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* __this, const RuntimeMethod* method)
{
	((  void (*) (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*, const RuntimeMethod*))List_1__ctor_m0AFBAEA7EC427E32CC9CA267B1930DC5DF67A374_gshared)((List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*)__this, method);
}
// System.Void System.Collections.Generic.List`1<System.String>::Add(T)
inline void List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline (List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* __this, String_t* ___item0, const RuntimeMethod* method)
{
	((  void (*) (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType*/Il2CppFullySharedGenericAny, const RuntimeMethod*))List_1_Add_mD4F3498FBD3BDD3F03CBCFB38041CBAC9C28CAFC_gshared_inline)((List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A*)__this, (Il2CppFullySharedGenericAny)___item0, method);
}
// System.Void UnityEngine.MonoBehaviour::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MonoBehaviour__ctor_m592DB0105CA0BC97AA1C5F4AD27B12D68A3B7C1E (MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71* __this, const RuntimeMethod* method);
// System.Char System.String::get_Chars(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar String_get_Chars_mC49DF0CD2D3BE7BE97B3AD9C995BE3094F8E36D3 (String_t* __this, int32_t ___index0, const RuntimeMethod* method);
// System.String System.Char::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Char_ToString_m2A308731F9577C06AF3C0901234E2EAC8327410C (Il2CppChar* __this, const RuntimeMethod* method);
// System.String System.String::Concat(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mAF2CE02CC0CB7460753D0A1A91CCF2B1E9804C5D (String_t* ___str00, String_t* ___str11, const RuntimeMethod* method);
// System.Int32 System.String::get_Length()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline (String_t* __this, const RuntimeMethod* method);
// System.Void UnityEngine.Events.UnityAction`2<UnityEngine.SceneManagement.Scene,UnityEngine.SceneManagement.LoadSceneMode>::.ctor(System.Object,System.IntPtr)
inline void UnityAction_2__ctor_m0E0C01B7056EB1CB1E6C6F4FC457EBCA3F6B0041 (UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A* __this, RuntimeObject* ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	((  void (*) (UnityAction_2_t742C43FA6EAABE0458C753DFE15FDDFAE01EA73F*, RuntimeObject*, intptr_t, const RuntimeMethod*))UnityAction_2__ctor_m17203366119014F4963976DF6B8E83DE49274252_gshared)((UnityAction_2_t742C43FA6EAABE0458C753DFE15FDDFAE01EA73F*)__this, ___object0, ___method1, method);
}
// System.Void UnityEngine.SceneManagement.SceneManager::add_sceneLoaded(UnityEngine.Events.UnityAction`2<UnityEngine.SceneManagement.Scene,UnityEngine.SceneManagement.LoadSceneMode>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SceneManager_add_sceneLoaded_mDE45940CCEC5D17EB92EB76DB8931E5483FBCD2C (UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A* ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.SceneManagement.SceneManager::remove_sceneLoaded(UnityEngine.Events.UnityAction`2<UnityEngine.SceneManagement.Scene,UnityEngine.SceneManagement.LoadSceneMode>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SceneManager_remove_sceneLoaded_m8840CC33052C4A09A52BF927C3738A7B66783155 (UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A* ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Object::DontDestroyOnLoad(UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object_DontDestroyOnLoad_m303AA1C4DC810349F285B4809E426CBBA8F834F9 (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C* ___target0, const RuntimeMethod* method);
// T UnityEngine.Component::GetComponent<UnityEngine.CanvasGroup>()
inline CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094* Component_GetComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_mA3B0428368982ED39ADEBB220EE67D1E99D8B2D2 (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method)
{
	CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094* il2cppRetVal;
	((  void (*) (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny*, const RuntimeMethod*))Component_GetComponent_TisIl2CppFullySharedGenericAny_m47CBDD147982125387F078ABBFDAAB92D397A6C2_gshared)((Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*)__this, (Il2CppFullySharedGenericAny*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// System.Boolean UnityEngine.Object::op_Implicit(UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Implicit_m18E1885C296CC868AC918101523697CFE6413C79 (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C* ___exists0, const RuntimeMethod* method);
// T UnityEngine.Component::GetComponentInChildren<UnityEngine.UI.Image>()
inline Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* Component_GetComponentInChildren_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_m22ACF33DC0AB281D8B1E18650516D0765006FE66 (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method)
{
	Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* il2cppRetVal;
	((  void (*) (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny*, const RuntimeMethod*))Component_GetComponentInChildren_TisIl2CppFullySharedGenericAny_m6C912B287F81A629FB1D697E7CEB80D3B940295F_gshared)((Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*)__this, (Il2CppFullySharedGenericAny*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// T UnityEngine.Component::GetComponent<UnityEngine.UI.Image>()
inline Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* Component_GetComponent_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_mE74EE63C85A63FC34DCFC631BC229207B420BC79 (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method)
{
	Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* il2cppRetVal;
	((  void (*) (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny*, const RuntimeMethod*))Component_GetComponent_TisIl2CppFullySharedGenericAny_m47CBDD147982125387F078ABBFDAAB92D397A6C2_gshared)((Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*)__this, (Il2CppFullySharedGenericAny*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// System.Void UnityEngine.CanvasGroup::set_alpha(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CanvasGroup_set_alpha_m5C06839316D948BB4F75ED72C87FA1F1A20C333F (CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094* __this, float ___value0, const RuntimeMethod* method);
// System.Collections.IEnumerator Fader::FadeIt()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Fader_FadeIt_m9A0F19C0561342ECC35B4C63D1C0F6D21263E513 (Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* __this, const RuntimeMethod* method);
// System.Void UnityEngine.Debug::LogWarning(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_LogWarning_mEF15C6B17CE4E1FA7E379CDB82CE40FCD89A3F28 (RuntimeObject* ___message0, const RuntimeMethod* method);
// System.Void Fader/<FadeIt>d__13::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CFadeItU3Ed__13__ctor_m4983806A6FB489F57E177DCD8B170905C370C907 (U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099* __this, int32_t ___U3CU3E1__state0, const RuntimeMethod* method);
// System.Single UnityEngine.Time::get_time()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Time_get_time_m0BEE9AACD0723FE414465B77C9C64D12263675F3 (const RuntimeMethod* method);
// System.Single Fader::newAlpha(System.Single,System.Int32,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Fader_newAlpha_m025F82F0F336A3AC99E1367DF9A1E1B3C02D3642 (Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* __this, float ___delta0, int32_t ___to1, float ___currAlpha2, const RuntimeMethod* method);
// System.Void UnityEngine.SceneManagement.SceneManager::LoadScene(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SceneManager_LoadScene_m7237839058F581BFCA0A79BB96F6F931469E43CF (String_t* ___sceneName0, const RuntimeMethod* method);
// System.Void Initiate::DoneFading()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Initiate_DoneFading_m4180FA79DDCCB3ADF33825C84055701D68428D17 (const RuntimeMethod* method);
// System.Void UnityEngine.Debug::Log(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_Log_m86567BCF22BBE7809747817453CACA0E41E68219 (RuntimeObject* ___message0, const RuntimeMethod* method);
// System.Void UnityEngine.GameObject::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameObject__ctor_m7D0340DE160786E6EFA8DABD39EC3B694DA30AAD (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method);
// System.Void UnityEngine.Object::set_name(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object_set_name_mC79E6DC8FFD72479C90F0C4CC7F42A0FEAF5AE47 (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C* __this, String_t* ___value0, const RuntimeMethod* method);
// T UnityEngine.GameObject::AddComponent<UnityEngine.Canvas>()
inline Canvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26* GameObject_AddComponent_TisCanvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26_m13C85FD585C0679530F8B35D0B39D965702FD0F5 (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method)
{
	RuntimeObject* il2cppRetVal = ((  RuntimeObject* (*) (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*, const RuntimeMethod*))GameObject_AddComponent_TisRuntimeObject_m69B93700FACCF372F5753371C6E8FB780800B824_gshared)((GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)__this, method);
	return (Canvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26*)il2cppRetVal;
}
// System.Void UnityEngine.Canvas::set_renderMode(UnityEngine.RenderMode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Canvas_set_renderMode_mD73E953F8A115CF469508448A00D0EDAFAF5AB47 (Canvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26* __this, int32_t ___value0, const RuntimeMethod* method);
// T UnityEngine.GameObject::AddComponent<Fader>()
inline Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* GameObject_AddComponent_TisFader_t4082384C0679E40CABDB8F1A51E0246989537D24_m05AEC75245A2C82F9D47A618CC0DE93E72102C3B (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method)
{
	RuntimeObject* il2cppRetVal = ((  RuntimeObject* (*) (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*, const RuntimeMethod*))GameObject_AddComponent_TisRuntimeObject_m69B93700FACCF372F5753371C6E8FB780800B824_gshared)((GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)__this, method);
	return (Fader_t4082384C0679E40CABDB8F1A51E0246989537D24*)il2cppRetVal;
}
// T UnityEngine.GameObject::AddComponent<UnityEngine.CanvasGroup>()
inline CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094* GameObject_AddComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_m1C004B58918BA839B892637D46D95AF04D69DADA (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method)
{
	RuntimeObject* il2cppRetVal = ((  RuntimeObject* (*) (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*, const RuntimeMethod*))GameObject_AddComponent_TisRuntimeObject_m69B93700FACCF372F5753371C6E8FB780800B824_gshared)((GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)__this, method);
	return (CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094*)il2cppRetVal;
}
// T UnityEngine.GameObject::AddComponent<UnityEngine.UI.Image>()
inline Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* GameObject_AddComponent_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_mA327C9E1CA12BC531D587E7567F2067B96E6B6A0 (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method)
{
	RuntimeObject* il2cppRetVal = ((  RuntimeObject* (*) (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*, const RuntimeMethod*))GameObject_AddComponent_TisRuntimeObject_m69B93700FACCF372F5753371C6E8FB780800B824_gshared)((GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)__this, method);
	return (Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E*)il2cppRetVal;
}
// T UnityEngine.GameObject::GetComponent<Fader>()
inline Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* GameObject_GetComponent_TisFader_t4082384C0679E40CABDB8F1A51E0246989537D24_mC0661A39B823BACE89B865B139AD471E8E5A3B18 (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method)
{
	Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* il2cppRetVal;
	((  void (*) (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny*, const RuntimeMethod*))GameObject_GetComponent_TisIl2CppFullySharedGenericAny_m1122128E432233EB251AECF734E2B72A42A2C194_gshared)((GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)__this, (Il2CppFullySharedGenericAny*)&il2cppRetVal, method);
	return il2cppRetVal;
}
// System.Void Fader::InitiateFader()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Fader_InitiateFader_m07AE039731CCE36D0E46B5BECF23F89D1E6BBF4E (Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* __this, const RuntimeMethod* method);
// System.Void UnityEngine.Vector3::.ctor(System.Single,System.Single,System.Single)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2* __this, float ___x0, float ___y1, float ___z2, const RuntimeMethod* method);
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void TurnManager/<RemoveDice>d__41::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CRemoveDiceU3Ed__41__ctor_m85BCCD35A6B6A98EB4338EE38006B0C15203FCDA (U3CRemoveDiceU3Ed__41_tF3DAB6BC24EFB53E73F50C0E34AB6F2644CC077A* __this, int32_t ___U3CU3E1__state0, const RuntimeMethod* method)
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		int32_t L_0 = ___U3CU3E1__state0;
		__this->___U3CU3E1__state_0 = L_0;
		return;
	}
}
// System.Void TurnManager/<RemoveDice>d__41::System.IDisposable.Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CRemoveDiceU3Ed__41_System_IDisposable_Dispose_mF6DA3BA8166FD7DEC765102E8BD56EC4725A2363 (U3CRemoveDiceU3Ed__41_tF3DAB6BC24EFB53E73F50C0E34AB6F2644CC077A* __this, const RuntimeMethod* method)
{
	{
		return;
	}
}
// System.Boolean TurnManager/<RemoveDice>d__41::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CRemoveDiceU3Ed__41_MoveNext_mCB9FA551BAA63E5C2DCB63CD2B990C4D2C57A527 (U3CRemoveDiceU3Ed__41_tF3DAB6BC24EFB53E73F50C0E34AB6F2644CC077A* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponent_TisDice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A_m91F6B03AAEFF32E02B3AC36981E9D444FD235085_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Dictionary_2_get_Item_mBEB955470239D94BCB11C23D263104688CC2595B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Dictionary_2_get_Item_mF08AF6105F6A747B98757834E9BE7FE975620925_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Enumerable_ToList_TisDice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A_m34AE9657FC95F123345470E6C2B585FC6CD58C19_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Enumerator_Dispose_m183F16152D45183DC6CE1A195F14D3EB174A560D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Enumerator_MoveNext_m8E6AC5FCF5864D5C53C3838A777B86D2A9F5FDF7_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Enumerator_get_Current_m789E5F6B4BF5820DBED7D75DB908232A6B38C182_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_GetEnumerator_m56D864FDB01A87F36472DFA6F97147CDA63DE0B0_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Remove_m6ECEFB43E9BD1D878B805AA46E41073EEA8F2C6F_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Remove_mCCE85D4D5326536C4B214C73D07030F4CCD18485_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_get_Count_mD2E0CF4E337A946034B00E365D8B3B66445B4044_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralD8431B5D5BBDD13458B95AC3252777089DFF7F0A);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* V_1 = NULL;
	Enumerator_t4FA7DAAD85BE6888B74F6C499212CB3463719F5D V_2;
	memset((&V_2), 0, sizeof(V_2));
	Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* V_3 = NULL;
	int32_t V_4 = 0;
	Exception_t* __last_unhandled_exception = 0;
	il2cpp::utils::ExceptionSupportStack<int32_t, 1> __leave_targets;
	U3CRemoveDiceU3Ed__41_tF3DAB6BC24EFB53E73F50C0E34AB6F2644CC077A* G_B5_0 = NULL;
	U3CRemoveDiceU3Ed__41_tF3DAB6BC24EFB53E73F50C0E34AB6F2644CC077A* G_B4_0 = NULL;
	List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* G_B6_0 = NULL;
	U3CRemoveDiceU3Ed__41_tF3DAB6BC24EFB53E73F50C0E34AB6F2644CC077A* G_B6_1 = NULL;
	{
		int32_t L_0 = __this->___U3CU3E1__state_0;
		V_0 = L_0;
		TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* L_1 = __this->___U3CU3E4__this_3;
		V_1 = L_1;
		int32_t L_2 = V_0;
		if (!L_2)
		{
			goto IL_0017;
		}
	}
	{
		int32_t L_3 = V_0;
		if ((((int32_t)L_3) == ((int32_t)1)))
		{
			goto IL_0092;
		}
	}
	{
		return (bool)0;
	}

IL_0017:
	{
		__this->___U3CU3E1__state_0 = (-1);
		// List<Dice> diceList = removeFrom == "player" ? scripts.statSummoner.addedPlayerDice[diceType] : scripts.statSummoner.addedEnemyDice[diceType];
		String_t* L_4 = __this->___removeFrom_2;
		bool L_5;
		L_5 = String_op_Equality_m0D685A924E5CD78078F248ED1726DA5A9D7D6AC0(L_4, _stringLiteralD8431B5D5BBDD13458B95AC3252777089DFF7F0A, NULL);
		G_B4_0 = __this;
		if (L_5)
		{
			G_B5_0 = __this;
			goto IL_004e;
		}
	}
	{
		TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* L_6 = V_1;
		NullCheck(L_6);
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_7 = L_6->___scripts_11;
		NullCheck(L_7);
		StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539* L_8 = L_7->___statSummoner_21;
		NullCheck(L_8);
		Dictionary_2_t71DC4111D9530C502ACEA7A17068D6DC7AF41689* L_9 = L_8->___addedEnemyDice_21;
		String_t* L_10 = __this->___diceType_4;
		NullCheck(L_9);
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_11;
		L_11 = Dictionary_2_get_Item_mBEB955470239D94BCB11C23D263104688CC2595B(L_9, L_10, Dictionary_2_get_Item_mBEB955470239D94BCB11C23D263104688CC2595B_RuntimeMethod_var);
		G_B6_0 = L_11;
		G_B6_1 = G_B4_0;
		goto IL_0069;
	}

IL_004e:
	{
		TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* L_12 = V_1;
		NullCheck(L_12);
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_13 = L_12->___scripts_11;
		NullCheck(L_13);
		StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539* L_14 = L_13->___statSummoner_21;
		NullCheck(L_14);
		Dictionary_2_t71DC4111D9530C502ACEA7A17068D6DC7AF41689* L_15 = L_14->___addedPlayerDice_20;
		String_t* L_16 = __this->___diceType_4;
		NullCheck(L_15);
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_17;
		L_17 = Dictionary_2_get_Item_mBEB955470239D94BCB11C23D263104688CC2595B(L_15, L_16, Dictionary_2_get_Item_mBEB955470239D94BCB11C23D263104688CC2595B_RuntimeMethod_var);
		G_B6_0 = L_17;
		G_B6_1 = G_B5_0;
	}

IL_0069:
	{
		NullCheck(G_B6_1);
		G_B6_1->___U3CdiceListU3E5__2_5 = G_B6_0;
		Il2CppCodeGenWriteBarrier((void**)(&G_B6_1->___U3CdiceListU3E5__2_5), (void*)G_B6_0);
		// yield return scripts.delays[0.45f];
		TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* L_18 = V_1;
		NullCheck(L_18);
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_19 = L_18->___scripts_11;
		NullCheck(L_19);
		Dictionary_2_t5826E39B66190EFF2D3EA81361D5FC4BB8D6B212* L_20 = L_19->___delays_26;
		NullCheck(L_20);
		WaitForSeconds_tF179DF251655B8DF044952E70A60DF4B358A3DD3* L_21;
		L_21 = Dictionary_2_get_Item_mF08AF6105F6A747B98757834E9BE7FE975620925(L_20, (0.449999988f), Dictionary_2_get_Item_mF08AF6105F6A747B98757834E9BE7FE975620925_RuntimeMethod_var);
		__this->___U3CU3E2__current_1 = L_21;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CU3E2__current_1), (void*)L_21);
		__this->___U3CU3E1__state_0 = 1;
		return (bool)1;
	}

IL_0092:
	{
		__this->___U3CU3E1__state_0 = (-1);
		// foreach (Dice dice in diceList.ToList()) {
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_22 = __this->___U3CdiceListU3E5__2_5;
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_23;
		L_23 = Enumerable_ToList_TisDice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A_m34AE9657FC95F123345470E6C2B585FC6CD58C19(L_22, Enumerable_ToList_TisDice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A_m34AE9657FC95F123345470E6C2B585FC6CD58C19_RuntimeMethod_var);
		NullCheck(L_23);
		Enumerator_t4FA7DAAD85BE6888B74F6C499212CB3463719F5D L_24;
		L_24 = List_1_GetEnumerator_m56D864FDB01A87F36472DFA6F97147CDA63DE0B0(L_23, List_1_GetEnumerator_m56D864FDB01A87F36472DFA6F97147CDA63DE0B0_RuntimeMethod_var);
		V_2 = L_24;
	}

IL_00aa:
	try
	{// begin try (depth: 1)
		{
			goto IL_0121;
		}

IL_00ac:
		{
			// foreach (Dice dice in diceList.ToList()) {
			Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_25;
			L_25 = Enumerator_get_Current_m789E5F6B4BF5820DBED7D75DB908232A6B38C182_inline((&V_2), Enumerator_get_Current_m789E5F6B4BF5820DBED7D75DB908232A6B38C182_RuntimeMethod_var);
			V_3 = L_25;
			// if (dice.diceType == diceType) {
			Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_26 = V_3;
			NullCheck(L_26);
			String_t* L_27 = L_26->___diceType_5;
			String_t* L_28 = __this->___diceType_4;
			bool L_29;
			L_29 = String_op_Equality_m0D685A924E5CD78078F248ED1726DA5A9D7D6AC0(L_27, L_28, NULL);
			if (!L_29)
			{
				goto IL_0121;
			}
		}

IL_00c7:
		{
			// scripts.diceSummoner.existingDice.Remove(dice.gameObject);
			TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* L_30 = V_1;
			NullCheck(L_30);
			Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_31 = L_30->___scripts_11;
			NullCheck(L_31);
			DiceSummoner_tA51E21ADF511DD948DD0B44C50A73CF2F8C29DBC* L_32 = L_31->___diceSummoner_18;
			NullCheck(L_32);
			List_1_tB951CE80B58D1BF9650862451D8DAD8C231F207B* L_33 = L_32->___existingDice_8;
			Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_34 = V_3;
			NullCheck(L_34);
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_35;
			L_35 = Component_get_gameObject_m57AEFBB14DB39EC476F740BA000E170355DE691B(L_34, NULL);
			NullCheck(L_33);
			bool L_36;
			L_36 = List_1_Remove_mCCE85D4D5326536C4B214C73D07030F4CCD18485(L_33, L_35, List_1_Remove_mCCE85D4D5326536C4B214C73D07030F4CCD18485_RuntimeMethod_var);
			// diceList.Remove(dice);
			List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_37 = __this->___U3CdiceListU3E5__2_5;
			Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_38 = V_3;
			NullCheck(L_37);
			bool L_39;
			L_39 = List_1_Remove_m6ECEFB43E9BD1D878B805AA46E41073EEA8F2C6F(L_37, L_38, List_1_Remove_m6ECEFB43E9BD1D878B805AA46E41073EEA8F2C6F_RuntimeMethod_var);
			// Destroy(dice.gameObject);
			Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_40 = V_3;
			NullCheck(L_40);
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_41;
			L_41 = Component_get_gameObject_m57AEFBB14DB39EC476F740BA000E170355DE691B(L_40, NULL);
			il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
			Object_Destroy_mFCDAE6333522488F60597AF019EA90BB1207A5AA(L_41, NULL);
			// scripts.diceSummoner.SaveDiceValues();
			TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* L_42 = V_1;
			NullCheck(L_42);
			Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_43 = L_42->___scripts_11;
			NullCheck(L_43);
			DiceSummoner_tA51E21ADF511DD948DD0B44C50A73CF2F8C29DBC* L_44 = L_43->___diceSummoner_18;
			NullCheck(L_44);
			DiceSummoner_SaveDiceValues_m1694DCB6AEF34961E3CEFD220911269D29007370(L_44, NULL);
			// scripts.statSummoner.SetDebugInformationFor(removeFrom);
			TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* L_45 = V_1;
			NullCheck(L_45);
			Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_46 = L_45->___scripts_11;
			NullCheck(L_46);
			StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539* L_47 = L_46->___statSummoner_21;
			String_t* L_48 = __this->___removeFrom_2;
			NullCheck(L_47);
			StatSummoner_SetDebugInformationFor_m9A63F7BD07E872EBD85AF47B9251D0A113B21FB5(L_47, L_48, NULL);
		}

IL_0121:
		{
			// foreach (Dice dice in diceList.ToList()) {
			bool L_49;
			L_49 = Enumerator_MoveNext_m8E6AC5FCF5864D5C53C3838A777B86D2A9F5FDF7((&V_2), Enumerator_MoveNext_m8E6AC5FCF5864D5C53C3838A777B86D2A9F5FDF7_RuntimeMethod_var);
			if (L_49)
			{
				goto IL_00ac;
			}
		}

IL_012a:
		{
			IL2CPP_LEAVE(0x314, FINALLY_012c);
		}
	}// end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t*)e.ex;
		goto FINALLY_012c;
	}

FINALLY_012c:
	{// begin finally (depth: 1)
		Enumerator_Dispose_m183F16152D45183DC6CE1A195F14D3EB174A560D((&V_2), Enumerator_Dispose_m183F16152D45183DC6CE1A195F14D3EB174A560D_RuntimeMethod_var);
		IL2CPP_END_FINALLY(300)
	}// end finally (depth: 1)
	IL2CPP_CLEANUP(300)
	{
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t*)
		IL2CPP_JUMP_TBL(0x314, IL_013a)
	}

IL_013a:
	{
		// for (int i = 0; i < diceList.Count; i++) {
		V_4 = 0;
		goto IL_0278;
	}

IL_0142:
	{
		// if (removeFrom == "player") {
		String_t* L_50 = __this->___removeFrom_2;
		bool L_51;
		L_51 = String_op_Equality_m0D685A924E5CD78078F248ED1726DA5A9D7D6AC0(L_50, _stringLiteralD8431B5D5BBDD13458B95AC3252777089DFF7F0A, NULL);
		if (!L_51)
		{
			goto IL_01cc;
		}
	}
	{
		// diceList[i].transform.position = new Vector2(scripts.statSummoner.OutermostPlayerX(diceType) - (diceList.Count) + (i * scripts.statSummoner.diceOffset), diceList[i].transform.position.y);
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_52 = __this->___U3CdiceListU3E5__2_5;
		int32_t L_53 = V_4;
		NullCheck(L_52);
		Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_54;
		L_54 = List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6(L_52, L_53, List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6_RuntimeMethod_var);
		NullCheck(L_54);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_55;
		L_55 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(L_54, NULL);
		TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* L_56 = V_1;
		NullCheck(L_56);
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_57 = L_56->___scripts_11;
		NullCheck(L_57);
		StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539* L_58 = L_57->___statSummoner_21;
		String_t* L_59 = __this->___diceType_4;
		NullCheck(L_58);
		float L_60;
		L_60 = StatSummoner_OutermostPlayerX_mBA0D0C3A9EB4A3026DFE44CD7A7E650341980C02(L_58, L_59, (String_t*)NULL, NULL);
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_61 = __this->___U3CdiceListU3E5__2_5;
		NullCheck(L_61);
		int32_t L_62;
		L_62 = List_1_get_Count_mD2E0CF4E337A946034B00E365D8B3B66445B4044_inline(L_61, List_1_get_Count_mD2E0CF4E337A946034B00E365D8B3B66445B4044_RuntimeMethod_var);
		int32_t L_63 = V_4;
		TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* L_64 = V_1;
		NullCheck(L_64);
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_65 = L_64->___scripts_11;
		NullCheck(L_65);
		StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539* L_66 = L_65->___statSummoner_21;
		NullCheck(L_66);
		float L_67 = L_66->___diceOffset_14;
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_68 = __this->___U3CdiceListU3E5__2_5;
		int32_t L_69 = V_4;
		NullCheck(L_68);
		Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_70;
		L_70 = List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6(L_68, L_69, List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6_RuntimeMethod_var);
		NullCheck(L_70);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_71;
		L_71 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(L_70, NULL);
		NullCheck(L_71);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_72;
		L_72 = Transform_get_position_m69CD5FA214FDAE7BB701552943674846C220FDE1(L_71, NULL);
		float L_73 = L_72.___y_3;
		Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 L_74;
		memset((&L_74), 0, sizeof(L_74));
		Vector2__ctor_m9525B79969AFFE3254B303A40997A56DEEB6F548_inline((&L_74), ((float)il2cpp_codegen_add((float)((float)il2cpp_codegen_subtract((float)L_60, (float)((float)((float)L_62)))), (float)((float)il2cpp_codegen_multiply((float)((float)((float)L_63)), (float)L_67)))), L_73, /*hidden argument*/NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_75;
		L_75 = Vector2_op_Implicit_mCD214B04BC52AED3C89C3BEF664B6247E5F8954A_inline(L_74, NULL);
		NullCheck(L_55);
		Transform_set_position_mA1A817124BB41B685043DED2A9BA48CDF37C4156(L_55, L_75, NULL);
		goto IL_0244;
	}

IL_01cc:
	{
		// diceList[i].transform.position = new Vector2(scripts.statSummoner.OutermostEnemyX(diceType) + (diceList.Count - 1) + (i * (-scripts.statSummoner.diceOffset)), diceList[i].transform.position.y);
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_76 = __this->___U3CdiceListU3E5__2_5;
		int32_t L_77 = V_4;
		NullCheck(L_76);
		Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_78;
		L_78 = List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6(L_76, L_77, List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6_RuntimeMethod_var);
		NullCheck(L_78);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_79;
		L_79 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(L_78, NULL);
		TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* L_80 = V_1;
		NullCheck(L_80);
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_81 = L_80->___scripts_11;
		NullCheck(L_81);
		StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539* L_82 = L_81->___statSummoner_21;
		String_t* L_83 = __this->___diceType_4;
		NullCheck(L_82);
		float L_84;
		L_84 = StatSummoner_OutermostEnemyX_m611089CE18D77FD22CDE74ED71CA364749DF384E(L_82, L_83, NULL);
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_85 = __this->___U3CdiceListU3E5__2_5;
		NullCheck(L_85);
		int32_t L_86;
		L_86 = List_1_get_Count_mD2E0CF4E337A946034B00E365D8B3B66445B4044_inline(L_85, List_1_get_Count_mD2E0CF4E337A946034B00E365D8B3B66445B4044_RuntimeMethod_var);
		int32_t L_87 = V_4;
		TurnManager_t21C9AF6CC20195E4C26912566DA600813FBB3B41* L_88 = V_1;
		NullCheck(L_88);
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_89 = L_88->___scripts_11;
		NullCheck(L_89);
		StatSummoner_t3483D9B733E47ABAD41680D265B5E95E66CC1539* L_90 = L_89->___statSummoner_21;
		NullCheck(L_90);
		float L_91 = L_90->___diceOffset_14;
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_92 = __this->___U3CdiceListU3E5__2_5;
		int32_t L_93 = V_4;
		NullCheck(L_92);
		Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_94;
		L_94 = List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6(L_92, L_93, List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6_RuntimeMethod_var);
		NullCheck(L_94);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_95;
		L_95 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(L_94, NULL);
		NullCheck(L_95);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_96;
		L_96 = Transform_get_position_m69CD5FA214FDAE7BB701552943674846C220FDE1(L_95, NULL);
		float L_97 = L_96.___y_3;
		Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 L_98;
		memset((&L_98), 0, sizeof(L_98));
		Vector2__ctor_m9525B79969AFFE3254B303A40997A56DEEB6F548_inline((&L_98), ((float)il2cpp_codegen_add((float)((float)il2cpp_codegen_add((float)L_84, (float)((float)((float)((int32_t)il2cpp_codegen_subtract((int32_t)L_86, (int32_t)1)))))), (float)((float)il2cpp_codegen_multiply((float)((float)((float)L_87)), (float)((-L_91)))))), L_97, /*hidden argument*/NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_99;
		L_99 = Vector2_op_Implicit_mCD214B04BC52AED3C89C3BEF664B6247E5F8954A_inline(L_98, NULL);
		NullCheck(L_79);
		Transform_set_position_mA1A817124BB41B685043DED2A9BA48CDF37C4156(L_79, L_99, NULL);
	}

IL_0244:
	{
		// diceList[i].GetComponent<Dice>().instantiationPos = diceList[i].transform.position;
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_100 = __this->___U3CdiceListU3E5__2_5;
		int32_t L_101 = V_4;
		NullCheck(L_100);
		Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_102;
		L_102 = List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6(L_100, L_101, List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6_RuntimeMethod_var);
		NullCheck(L_102);
		Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_103;
		L_103 = Component_GetComponent_TisDice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A_m91F6B03AAEFF32E02B3AC36981E9D444FD235085(L_102, Component_GetComponent_TisDice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A_m91F6B03AAEFF32E02B3AC36981E9D444FD235085_RuntimeMethod_var);
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_104 = __this->___U3CdiceListU3E5__2_5;
		int32_t L_105 = V_4;
		NullCheck(L_104);
		Dice_t0E92A2DB9F3BE47801348369649C47FC15F1B42A* L_106;
		L_106 = List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6(L_104, L_105, List_1_get_Item_m2B2BCCD44425A0DC5FC5BB686CEDD0C1AFD817A6_RuntimeMethod_var);
		NullCheck(L_106);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_107;
		L_107 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(L_106, NULL);
		NullCheck(L_107);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_108;
		L_108 = Transform_get_position_m69CD5FA214FDAE7BB701552943674846C220FDE1(L_107, NULL);
		NullCheck(L_103);
		L_103->___instantiationPos_11 = L_108;
		// for (int i = 0; i < diceList.Count; i++) {
		int32_t L_109 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add((int32_t)L_109, (int32_t)1));
	}

IL_0278:
	{
		// for (int i = 0; i < diceList.Count; i++) {
		int32_t L_110 = V_4;
		List_1_t4E5239002FA713FF02F1A18A9E87FD1BE8707789* L_111 = __this->___U3CdiceListU3E5__2_5;
		NullCheck(L_111);
		int32_t L_112;
		L_112 = List_1_get_Count_mD2E0CF4E337A946034B00E365D8B3B66445B4044_inline(L_111, List_1_get_Count_mD2E0CF4E337A946034B00E365D8B3B66445B4044_RuntimeMethod_var);
		if ((((int32_t)L_110) < ((int32_t)L_112)))
		{
			goto IL_0142;
		}
	}
	{
		// }
		return (bool)0;
	}
}
// System.Object TurnManager/<RemoveDice>d__41::System.Collections.Generic.IEnumerator<System.Object>.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* U3CRemoveDiceU3Ed__41_System_Collections_Generic_IEnumeratorU3CSystem_ObjectU3E_get_Current_m60434E153B8DBA9DFE291177789D613314B6BB5D (U3CRemoveDiceU3Ed__41_tF3DAB6BC24EFB53E73F50C0E34AB6F2644CC077A* __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = __this->___U3CU3E2__current_1;
		return L_0;
	}
}
// System.Void TurnManager/<RemoveDice>d__41::System.Collections.IEnumerator.Reset()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CRemoveDiceU3Ed__41_System_Collections_IEnumerator_Reset_mC09053FA9A21B90ED860BFE6AD8A76F2772507C0 (U3CRemoveDiceU3Ed__41_tF3DAB6BC24EFB53E73F50C0E34AB6F2644CC077A* __this, const RuntimeMethod* method)
{
	{
		NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A* L_0 = (NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A_il2cpp_TypeInfo_var)));
		NotSupportedException__ctor_m1398D0CDE19B36AA3DE9392879738C1EA2439CDF(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&U3CRemoveDiceU3Ed__41_System_Collections_IEnumerator_Reset_mC09053FA9A21B90ED860BFE6AD8A76F2772507C0_RuntimeMethod_var)));
	}
}
// System.Object TurnManager/<RemoveDice>d__41::System.Collections.IEnumerator.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* U3CRemoveDiceU3Ed__41_System_Collections_IEnumerator_get_Current_mF3BC7876D8898E73EA5675EE2DC464E6FB7AF544 (U3CRemoveDiceU3Ed__41_tF3DAB6BC24EFB53E73F50C0E34AB6F2644CC077A* __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = __this->___U3CU3E2__current_1;
		return L_0;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Tutorial::Start()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tutorial_Start_m799566239F76DABF8C502C786D3480BFD74D597A (Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponent_TisSpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B_m6181F10C09FC1650DAE0EF2308D344A2F170AA45_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_FindObjectOfType_TisScripts_tA1F67F81394769A4746B08F203BBD942A423AF5C_mD4A9D0F71D72CAEF1369857214457214E426E335_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		// scripts = FindObjectOfType<Scripts>();
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_0;
		L_0 = Object_FindObjectOfType_TisScripts_tA1F67F81394769A4746B08F203BBD942A423AF5C_mD4A9D0F71D72CAEF1369857214457214E426E335(Object_FindObjectOfType_TisScripts_tA1F67F81394769A4746B08F203BBD942A423AF5C_mD4A9D0F71D72CAEF1369857214457214E426E335_RuntimeMethod_var);
		__this->___scripts_8 = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___scripts_8), (void*)L_0);
		// GetComponent<SpriteRenderer>().sprite = blackBox;
		SpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B* L_1;
		L_1 = Component_GetComponent_TisSpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B_m6181F10C09FC1650DAE0EF2308D344A2F170AA45(__this, Component_GetComponent_TisSpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B_m6181F10C09FC1650DAE0EF2308D344A2F170AA45_RuntimeMethod_var);
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_2 = __this->___blackBox_4;
		NullCheck(L_1);
		SpriteRenderer_set_sprite_m7B176E33955108C60CAE21DFC153A0FAC674CB53(L_1, L_2, NULL);
		// mainScroll = StartCoroutine(TextAnimation(0));
		RuntimeObject* L_3;
		L_3 = Tutorial_TextAnimation_m1720AF20B6881F0494F69C87B96669A98391ED47(__this, 0, NULL);
		Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* L_4;
		L_4 = MonoBehaviour_StartCoroutine_m4CAFF732AA28CD3BDC5363B44A863575530EC812(__this, L_3, NULL);
		__this->___mainScroll_11 = L_4;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___mainScroll_11), (void*)L_4);
		// }
		return;
	}
}
// System.Void Tutorial::OnMouseDown()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tutorial_OnMouseDown_mC45CA500E3675195C1F214D451C979ADD84BAB87 (Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponent_TisSpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B_m6181F10C09FC1650DAE0EF2308D344A2F170AA45_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_get_Count_m4C37ED2D928D63B80F55AF434730C2D64EEB9F22_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_get_Count_mB63183A9151F4345A9DD444A7CBE0D6E03F77C7C_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_get_Item_m21AEC50E791371101DC22ABCF96A2E46800811F8_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral1205791EEFC8465A8944A7B058812B718E7F0909);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE12AF4099C3E5EC3FE1AB438E75CC43EEA1E65C6);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE9923573DCC65C12B03FC7B779BF1A80B345C19E);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	il2cpp::utils::ExceptionSupportStack<int32_t, 4> __leave_targets;
	{
		// scripts.soundManager.PlayClip("click0");
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_0 = __this->___scripts_8;
		NullCheck(L_0);
		SoundManager_tCA2CCAC5CDF1BA10E525C01C8D1D0DFAC9BE3734* L_1 = L_0->___soundManager_19;
		NullCheck(L_1);
		SoundManager_PlayClip_m45E4AD5F009E3F6679345FF5BAECF1B20F442C58(L_1, _stringLiteral1205791EEFC8465A8944A7B058812B718E7F0909, NULL);
		// if (curIndex + 1 < tutorialTextList.Count) {
		int32_t L_2 = __this->___curIndex_7;
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_3 = __this->___tutorialTextList_13;
		NullCheck(L_3);
		int32_t L_4;
		L_4 = List_1_get_Count_mB63183A9151F4345A9DD444A7CBE0D6E03F77C7C_inline(L_3, List_1_get_Count_mB63183A9151F4345A9DD444A7CBE0D6E03F77C7C_RuntimeMethod_var);
		if ((((int32_t)((int32_t)il2cpp_codegen_add((int32_t)L_2, (int32_t)1))) >= ((int32_t)L_4)))
		{
			goto IL_0157;
		}
	}
	{
		// if (isAnimating) {
		bool L_5 = __this->___isAnimating_10;
		if (!L_5)
		{
			goto IL_00c0;
		}
	}

IL_0038:
	try
	{// begin try (depth: 1)
		// try { StopCoroutine(mainScroll); } catch { }
		Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* L_6 = __this->___mainScroll_11;
		MonoBehaviour_StopCoroutine_mB0FC91BE84203BD8E360B3FBAE5B958B4C5ED22A(__this, L_6, NULL);
		// try { StopCoroutine(mainScroll); } catch { }
		goto IL_0049;
	}// end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&RuntimeObject_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_0046;
		}
		throw e;
	}

CATCH_0046:
	{// begin catch(System.Object)
		// try { StopCoroutine(mainScroll); } catch { }
		// try { StopCoroutine(mainScroll); } catch { }
		IL2CPP_POP_ACTIVE_EXCEPTION();
		goto IL_0049;
	}// end catch (depth: 1)

IL_0049:
	{
	}

IL_004a:
	try
	{// begin try (depth: 1)
		// try { StopCoroutine(statScroll); } catch { }
		Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* L_7 = __this->___statScroll_12;
		MonoBehaviour_StopCoroutine_mB0FC91BE84203BD8E360B3FBAE5B958B4C5ED22A(__this, L_7, NULL);
		// try { StopCoroutine(statScroll); } catch { }
		goto IL_005b;
	}// end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&RuntimeObject_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_0058;
		}
		throw e;
	}

CATCH_0058:
	{// begin catch(System.Object)
		// try { StopCoroutine(statScroll); } catch { }
		// try { StopCoroutine(statScroll); } catch { }
		IL2CPP_POP_ACTIVE_EXCEPTION();
		goto IL_005b;
	}// end catch (depth: 1)

IL_005b:
	{
		// isAnimating = false;
		__this->___isAnimating_10 = (bool)0;
		// if (curIndex == 3) {
		int32_t L_8 = __this->___curIndex_7;
		if ((!(((uint32_t)L_8) == ((uint32_t)3))))
		{
			goto IL_0089;
		}
	}
	{
		// GetComponent<SpriteRenderer>().sprite = null;
		SpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B* L_9;
		L_9 = Component_GetComponent_TisSpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B_m6181F10C09FC1650DAE0EF2308D344A2F170AA45(__this, Component_GetComponent_TisSpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B_m6181F10C09FC1650DAE0EF2308D344A2F170AA45_RuntimeMethod_var);
		NullCheck(L_9);
		SpriteRenderer_set_sprite_m7B176E33955108C60CAE21DFC153A0FAC674CB53(L_9, (Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99*)NULL, NULL);
		// statText.text = "< These are your weapon stats.";
		TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* L_10 = __this->___statText_6;
		NullCheck(L_10);
		VirtualActionInvoker1< String_t* >::Invoke(66 /* System.Void TMPro.TMP_Text::set_text(System.String) */, L_10, _stringLiteralE12AF4099C3E5EC3FE1AB438E75CC43EEA1E65C6);
		goto IL_00a2;
	}

IL_0089:
	{
		// else if (curIndex == 4) { statText.text = "< Accuracy\n< Speed\n< Damage\n< Parry"; }
		int32_t L_11 = __this->___curIndex_7;
		if ((!(((uint32_t)L_11) == ((uint32_t)4))))
		{
			goto IL_00a2;
		}
	}
	{
		// else if (curIndex == 4) { statText.text = "< Accuracy\n< Speed\n< Damage\n< Parry"; }
		TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* L_12 = __this->___statText_6;
		NullCheck(L_12);
		VirtualActionInvoker1< String_t* >::Invoke(66 /* System.Void TMPro.TMP_Text::set_text(System.String) */, L_12, _stringLiteralE9923573DCC65C12B03FC7B779BF1A80B345C19E);
	}

IL_00a2:
	{
		// tutorialText.text = tutorialTextList[curIndex];
		TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* L_13 = __this->___tutorialText_5;
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_14 = __this->___tutorialTextList_13;
		int32_t L_15 = __this->___curIndex_7;
		NullCheck(L_14);
		String_t* L_16;
		L_16 = List_1_get_Item_m21AEC50E791371101DC22ABCF96A2E46800811F8(L_14, L_15, List_1_get_Item_m21AEC50E791371101DC22ABCF96A2E46800811F8_RuntimeMethod_var);
		NullCheck(L_13);
		VirtualActionInvoker1< String_t* >::Invoke(66 /* System.Void TMPro.TMP_Text::set_text(System.String) */, L_13, L_16);
		goto IL_00ef;
	}

IL_00c0:
	{
		// if (curIndex is not (2 or 12 or 13 or 19 or 20 or 21 or 22)) { Increment(); }
		int32_t L_17 = __this->___curIndex_7;
		V_0 = L_17;
		int32_t L_18 = V_0;
		if ((((int32_t)L_18) == ((int32_t)2)))
		{
			goto IL_00ef;
		}
	}
	{
		int32_t L_19 = V_0;
		if ((((int32_t)L_19) == ((int32_t)((int32_t)12))))
		{
			goto IL_00ef;
		}
	}
	{
		int32_t L_20 = V_0;
		if ((((int32_t)L_20) == ((int32_t)((int32_t)13))))
		{
			goto IL_00ef;
		}
	}
	{
		int32_t L_21 = V_0;
		if ((((int32_t)L_21) == ((int32_t)((int32_t)19))))
		{
			goto IL_00ef;
		}
	}
	{
		int32_t L_22 = V_0;
		if ((((int32_t)L_22) == ((int32_t)((int32_t)20))))
		{
			goto IL_00ef;
		}
	}
	{
		int32_t L_23 = V_0;
		if ((((int32_t)L_23) == ((int32_t)((int32_t)21))))
		{
			goto IL_00ef;
		}
	}
	{
		int32_t L_24 = V_0;
		if ((((int32_t)L_24) == ((int32_t)((int32_t)22))))
		{
			goto IL_00ef;
		}
	}
	{
		// if (curIndex is not (2 or 12 or 13 or 19 or 20 or 21 or 22)) { Increment(); }
		Tutorial_Increment_mF12EB763F6A8E8375CEF37B46981936AE9DF3276(__this, NULL);
	}

IL_00ef:
	{
		// if (curIndex == 12 && scripts.diceSummoner.existingDice.Count == 0) { scripts.diceSummoner.SummonDice(true, false);  }
		int32_t L_25 = __this->___curIndex_7;
		if ((!(((uint32_t)L_25) == ((uint32_t)((int32_t)12)))))
		{
			goto IL_0123;
		}
	}
	{
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_26 = __this->___scripts_8;
		NullCheck(L_26);
		DiceSummoner_tA51E21ADF511DD948DD0B44C50A73CF2F8C29DBC* L_27 = L_26->___diceSummoner_18;
		NullCheck(L_27);
		List_1_tB951CE80B58D1BF9650862451D8DAD8C231F207B* L_28 = L_27->___existingDice_8;
		NullCheck(L_28);
		int32_t L_29;
		L_29 = List_1_get_Count_m4C37ED2D928D63B80F55AF434730C2D64EEB9F22_inline(L_28, List_1_get_Count_m4C37ED2D928D63B80F55AF434730C2D64EEB9F22_RuntimeMethod_var);
		if (L_29)
		{
			goto IL_0123;
		}
	}
	{
		// if (curIndex == 12 && scripts.diceSummoner.existingDice.Count == 0) { scripts.diceSummoner.SummonDice(true, false);  }
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_30 = __this->___scripts_8;
		NullCheck(L_30);
		DiceSummoner_tA51E21ADF511DD948DD0B44C50A73CF2F8C29DBC* L_31 = L_30->___diceSummoner_18;
		NullCheck(L_31);
		DiceSummoner_SummonDice_mED35D68D43E07F5F6097C300597DEC8D998D1A33(L_31, (bool)1, (bool)0, NULL);
		return;
	}

IL_0123:
	{
		// else if (curIndex == 21) { preventAttack = false; }
		int32_t L_32 = __this->___curIndex_7;
		if ((!(((uint32_t)L_32) == ((uint32_t)((int32_t)21)))))
		{
			goto IL_0135;
		}
	}
	{
		// else if (curIndex == 21) { preventAttack = false; }
		__this->___preventAttack_9 = (bool)0;
		return;
	}

IL_0135:
	{
		// else if (curIndex != 3 && curIndex != 4) { statText.text = ""; }
		int32_t L_33 = __this->___curIndex_7;
		if ((((int32_t)L_33) == ((int32_t)3)))
		{
			goto IL_0157;
		}
	}
	{
		int32_t L_34 = __this->___curIndex_7;
		if ((((int32_t)L_34) == ((int32_t)4)))
		{
			goto IL_0157;
		}
	}
	{
		// else if (curIndex != 3 && curIndex != 4) { statText.text = ""; }
		TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* L_35 = __this->___statText_6;
		NullCheck(L_35);
		VirtualActionInvoker1< String_t* >::Invoke(66 /* System.Void TMPro.TMP_Text::set_text(System.String) */, L_35, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709);
	}

IL_0157:
	{
		// }
		return;
	}
}
// System.Void Tutorial::Increment()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tutorial_Increment_mF12EB763F6A8E8375CEF37B46981936AE9DF3276 (Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponent_TisSpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B_m6181F10C09FC1650DAE0EF2308D344A2F170AA45_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE12AF4099C3E5EC3FE1AB438E75CC43EEA1E65C6);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE9923573DCC65C12B03FC7B779BF1A80B345C19E);
		s_Il2CppMethodInitialized = true;
	}
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	il2cpp::utils::ExceptionSupportStack<int32_t, 4> __leave_targets;
	{
		// curIndex++;
		int32_t L_0 = __this->___curIndex_7;
		__this->___curIndex_7 = ((int32_t)il2cpp_codegen_add((int32_t)L_0, (int32_t)1));
	}

IL_000e:
	try
	{// begin try (depth: 1)
		// try { StopCoroutine(mainScroll); } catch {}
		Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* L_1 = __this->___mainScroll_11;
		MonoBehaviour_StopCoroutine_mB0FC91BE84203BD8E360B3FBAE5B958B4C5ED22A(__this, L_1, NULL);
		// try { StopCoroutine(mainScroll); } catch {}
		goto IL_001f;
	}// end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&RuntimeObject_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_001c;
		}
		throw e;
	}

CATCH_001c:
	{// begin catch(System.Object)
		// try { StopCoroutine(mainScroll); } catch {}
		// try { StopCoroutine(mainScroll); } catch {}
		IL2CPP_POP_ACTIVE_EXCEPTION();
		goto IL_001f;
	}// end catch (depth: 1)

IL_001f:
	{
	}

IL_0020:
	try
	{// begin try (depth: 1)
		// try { StopCoroutine(statScroll); } catch {}
		Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* L_2 = __this->___statScroll_12;
		MonoBehaviour_StopCoroutine_mB0FC91BE84203BD8E360B3FBAE5B958B4C5ED22A(__this, L_2, NULL);
		// try { StopCoroutine(statScroll); } catch {}
		goto IL_0031;
	}// end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&RuntimeObject_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_002e;
		}
		throw e;
	}

CATCH_002e:
	{// begin catch(System.Object)
		// try { StopCoroutine(statScroll); } catch {}
		// try { StopCoroutine(statScroll); } catch {}
		IL2CPP_POP_ACTIVE_EXCEPTION();
		goto IL_0031;
	}// end catch (depth: 1)

IL_0031:
	{
		// statText.text = "";
		TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* L_3 = __this->___statText_6;
		NullCheck(L_3);
		VirtualActionInvoker1< String_t* >::Invoke(66 /* System.Void TMPro.TMP_Text::set_text(System.String) */, L_3, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709);
		// tutorialText.text = "";
		TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* L_4 = __this->___tutorialText_5;
		NullCheck(L_4);
		VirtualActionInvoker1< String_t* >::Invoke(66 /* System.Void TMPro.TMP_Text::set_text(System.String) */, L_4, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709);
		// if (curIndex == 3) {
		int32_t L_5 = __this->___curIndex_7;
		if ((!(((uint32_t)L_5) == ((uint32_t)3))))
		{
			goto IL_007f;
		}
	}
	{
		// GetComponent<SpriteRenderer>().sprite = null;
		SpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B* L_6;
		L_6 = Component_GetComponent_TisSpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B_m6181F10C09FC1650DAE0EF2308D344A2F170AA45(__this, Component_GetComponent_TisSpriteRenderer_t1DD7FE258F072E1FA87D6577BA27225892B8047B_m6181F10C09FC1650DAE0EF2308D344A2F170AA45_RuntimeMethod_var);
		NullCheck(L_6);
		SpriteRenderer_set_sprite_m7B176E33955108C60CAE21DFC153A0FAC674CB53(L_6, (Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99*)NULL, NULL);
		// statScroll = StartCoroutine(TextAnimation("< These are your weapon stats."));
		RuntimeObject* L_7;
		L_7 = Tutorial_TextAnimation_mAC267160B03FE9932679CA6E9E1BF719AFDC32ED(__this, _stringLiteralE12AF4099C3E5EC3FE1AB438E75CC43EEA1E65C6, NULL);
		Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* L_8;
		L_8 = MonoBehaviour_StartCoroutine_m4CAFF732AA28CD3BDC5363B44A863575530EC812(__this, L_7, NULL);
		__this->___statScroll_12 = L_8;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___statScroll_12), (void*)L_8);
		goto IL_009f;
	}

IL_007f:
	{
		// else if (curIndex == 4) { statScroll = StartCoroutine(TextAnimation("< Accuracy\n< Speed\n< Damage\n< Parry")); }
		int32_t L_9 = __this->___curIndex_7;
		if ((!(((uint32_t)L_9) == ((uint32_t)4))))
		{
			goto IL_009f;
		}
	}
	{
		// else if (curIndex == 4) { statScroll = StartCoroutine(TextAnimation("< Accuracy\n< Speed\n< Damage\n< Parry")); }
		RuntimeObject* L_10;
		L_10 = Tutorial_TextAnimation_mAC267160B03FE9932679CA6E9E1BF719AFDC32ED(__this, _stringLiteralE9923573DCC65C12B03FC7B779BF1A80B345C19E, NULL);
		Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* L_11;
		L_11 = MonoBehaviour_StartCoroutine_m4CAFF732AA28CD3BDC5363B44A863575530EC812(__this, L_10, NULL);
		__this->___statScroll_12 = L_11;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___statScroll_12), (void*)L_11);
	}

IL_009f:
	{
		// mainScroll = StartCoroutine(TextAnimation(curIndex));
		int32_t L_12 = __this->___curIndex_7;
		RuntimeObject* L_13;
		L_13 = Tutorial_TextAnimation_m1720AF20B6881F0494F69C87B96669A98391ED47(__this, L_12, NULL);
		Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* L_14;
		L_14 = MonoBehaviour_StartCoroutine_m4CAFF732AA28CD3BDC5363B44A863575530EC812(__this, L_13, NULL);
		__this->___mainScroll_11 = L_14;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___mainScroll_11), (void*)L_14);
		// }
		return;
	}
}
// System.Collections.IEnumerator Tutorial::TextAnimation(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Tutorial_TextAnimation_m1720AF20B6881F0494F69C87B96669A98391ED47 (Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* __this, int32_t ___index0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02* L_0 = (U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02*)il2cpp_codegen_object_new(U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02_il2cpp_TypeInfo_var);
		U3CTextAnimationU3Ed__13__ctor_mDA59BE84AEE7C584D38F61046A3EE671D28D4C4F(L_0, 0, /*hidden argument*/NULL);
		U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02* L_1 = L_0;
		NullCheck(L_1);
		L_1->___U3CU3E4__this_2 = __this;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___U3CU3E4__this_2), (void*)__this);
		U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02* L_2 = L_1;
		int32_t L_3 = ___index0;
		NullCheck(L_2);
		L_2->___index_3 = L_3;
		return L_2;
	}
}
// System.Collections.IEnumerator Tutorial::TextAnimation(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Tutorial_TextAnimation_mAC267160B03FE9932679CA6E9E1BF719AFDC32ED (Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* __this, String_t* ___str0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8* L_0 = (U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8*)il2cpp_codegen_object_new(U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8_il2cpp_TypeInfo_var);
		U3CTextAnimationU3Ed__14__ctor_m5AD9D83A2EB7D2C21A3F746E47C8FE50FBB7AA5A(L_0, 0, /*hidden argument*/NULL);
		U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8* L_1 = L_0;
		NullCheck(L_1);
		L_1->___U3CU3E4__this_2 = __this;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___U3CU3E4__this_2), (void*)__this);
		U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8* L_2 = L_1;
		String_t* L_3 = ___str0;
		NullCheck(L_2);
		L_2->___str_3 = L_3;
		Il2CppCodeGenWriteBarrier((void**)(&L_2->___str_3), (void*)L_3);
		return L_2;
	}
}
// System.Void Tutorial::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tutorial__ctor_mCD41B6145B228A85EAAD173736F4BFD758605728 (Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral0DA8B99A80456CF0447EE4E9C8076CC92116C70F);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral1229F9FF79387730D4FE96408933191647B39278);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral267C2561791B8066965291F6BF4AC3AD56744BBC);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral630D82DDB91BD0A38D5F5BE931D8D7433736DCDC);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral73D105A88284D23B2E558768E82867E4EDBEE9C0);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral83A93055434B3DEFBA1FDCD5E3FE2A6CFFCB4D34);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral89BF96A86F8214B4D8D2B1FAC0423C4D55046B94);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral89E8BF424EDFA2DED494C2FF771626C991B4968F);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral8E55E46B8E8F02700ABAE1E9030A9CEEE3D96575);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral988F93A95ABFE53FF853FD26CE410B82A3E23BE3);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral9C04205D437A60FD17FE5555B3502042C6A436F3);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralA199A05EF4B94C954675DBB8F451DCE18F37C004);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralB71034B75D3E4B918B899344D72D11E843F82F1D);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralBADE01FA72995907D8CBB52ED0948028121B2A9E);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralBD6390C3D498E36B80DABABBC961431C400B0719);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralD3E06A4C4A311F489EB979283DBDC7CBB6CB9CF0);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralD4E298E8840B5A18F01203A40A9ABA9D222F56FE);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE1A596C859D35600B5AA7D4F585C3A76013BA55D);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralEB6A7D6D86E053EAEC72B09FACC475E0F45D4E42);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralECF501476651A1D856C0760EB5C4004165FDCD87);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralF330F1A44EDD893918E4984ECCD79A10D13E93EF);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralF680D63E130D5BE4EB8EAF5C9C69BFD423331BD0);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralFA9A9682704E0A907999201CAF9437D0F2E581AB);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralFE6476BE639D4C086B597DB56C0A679D5B634C5A);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public bool preventAttack = true;
		__this->___preventAttack_9 = (bool)1;
		// private readonly List<string> tutorialTextList = new() {
		//     "Welcome to MALLEUS GOBLINIFICARIUM,\na realistic dice-based combat simulator!\n\nYes, a realistic game can contain goblins and scrolls.\n[click] to continue",
		//     // ^ 0
		//     "You can navigate your inventory using\nleft and right [arrow_keys]\nor [left_mouse]\n\n[click] to continue",
		//     // ^ 1
		//     "Press [enter] or [left_mouse] to use an item,\n[shift_enter] or [right_mouse] to drop it.\nYou can use or drop selected items only.\n\n[drop the scroll] to continue",
		//     // ^ 2
		//     "\n\n\n\n[click] to continue",
		//     // ^ 3
		//     "\n\n\n\n[click] to continue",
		//     // ^ 4
		//     "Accuracy (green) allows you to choose\ndifferent body parts (each applying some\nspecial debuff in damaged) as a target.\n\n[click] to continue",
		//     // ^ 5
		//     "You can use up and down\n[arrow_keys] or [mouse_wheel]\nto adjust your aim.\n\n[click] to continue",
		//     // ^ 6
		//     "As you invest into your accuracy,\nyou will have more options\nto choose from.\n\n[click] to continue",
		//     // ^ 7
		//     "If your damage (red) is higher than\nthe enemy's parry (white), you will wound\nthe body part you are targeting.\n\n[click] to continue",
		//     // ^ 8
		//     "Respectively, if your parry (white) isn\nhigher than the enemy's damage (red), his\nattack will deal no harm.\n\n[click] to continue",
		//     // ^ 9
		//     "Speed (blue) defines who will strike first.\nIt matters because all debuffs are\napplied instantly.\n\n[click] to continue",
		//     // ^ 10
		//     "Speed also defines who will start\nthe draft, a process of picking dice,\none by one.\n\n[click] to continue",
		//     // ^ 11
		//     "The die you pick increases\nthe corresponding combat stat.\nYour speed is higher, so choose one!\n\n[click + drag die] to continue",
		//     // ^ 12
		//     "You can notice that your enemy has\ntaken a die as well.\nThis is how draft works.\n\n[pick 2 more dice] to continue",
		//     // ^ 13
		//     "Now your damage (7) is higher than\nenemy's parry (3), so you'll inflict a\nwound. You are also safe from his blow.\n\n[click] to continue",
		//     // ^ 14
		//     "You have the same speed (3)\nas your enemy,which still means you'll\nstrike first.\n\n[click] to continue",
		//     // ^ 15
		//     "You can probably start the fight now\nand land a successful hit. Enemy will\ndie once he has 3 wounds.\n\n[click] to continue",
		//     // ^ 16
		//     "But it would be wise to use your\nfirst-strike advantage and kill him in\none blow, before he can retaliate!\n\n[click] to continue",
		//     // ^ 17
		//     "The yellow icon above the aim list\nindicates your stamina, a resource\nused to increase your weapon stats.\n\n[click] to continue",
		//     // ^ 18
		//     "\nUse \"-\" and \"+\" to the left\nfrom your weapon stats to throw\n3 stamina into your accuracy (green)",
		//     // ^ 19
		//     "\nAim to the face, since it's\nthe only wound that is instantly lethal.\nScroll to the bottom of the aim list\nusing [arrow_keys] or [mouse_wheel]",
		//     // ^ 20
		//     "\n\nNow use your weapon\n(click the \"sword\" icon) to start the\nfight, and watch him die...",
		//     // ^ 21
		//     "Take the loot now. use [ctrl]\n or [left_mouse] to access his\ninventory.\n\n[take steak] to continue",
		//     // ^ 22
		//     "To the right you can see the stats of\nthe weapon you are about to take.\nYou can't carry more than one weapon.\n\n[click] to continue",
		//     // ^ 23
		//     "Finally, use the arrow in the\nenemy's inventory to proceed.\n\nThose are the very basics of\nMalleus Goblinificarium. \nYou'll learn more as you play more!"
		//     // ^ 24
		// };
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_0 = (List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD*)il2cpp_codegen_object_new(List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD_il2cpp_TypeInfo_var);
		List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E(L_0, /*hidden argument*/List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_1 = L_0;
		NullCheck(L_1);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_1, _stringLiteralFE6476BE639D4C086B597DB56C0A679D5B634C5A, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_2 = L_1;
		NullCheck(L_2);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_2, _stringLiteralB71034B75D3E4B918B899344D72D11E843F82F1D, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_3 = L_2;
		NullCheck(L_3);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_3, _stringLiteralE1A596C859D35600B5AA7D4F585C3A76013BA55D, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_4 = L_3;
		NullCheck(L_4);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_4, _stringLiteralFA9A9682704E0A907999201CAF9437D0F2E581AB, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_5 = L_4;
		NullCheck(L_5);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_5, _stringLiteralFA9A9682704E0A907999201CAF9437D0F2E581AB, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_6 = L_5;
		NullCheck(L_6);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_6, _stringLiteral988F93A95ABFE53FF853FD26CE410B82A3E23BE3, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_7 = L_6;
		NullCheck(L_7);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_7, _stringLiteralF330F1A44EDD893918E4984ECCD79A10D13E93EF, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_8 = L_7;
		NullCheck(L_8);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_8, _stringLiteral1229F9FF79387730D4FE96408933191647B39278, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_9 = L_8;
		NullCheck(L_9);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_9, _stringLiteralBADE01FA72995907D8CBB52ED0948028121B2A9E, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_10 = L_9;
		NullCheck(L_10);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_10, _stringLiteral267C2561791B8066965291F6BF4AC3AD56744BBC, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_11 = L_10;
		NullCheck(L_11);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_11, _stringLiteral8E55E46B8E8F02700ABAE1E9030A9CEEE3D96575, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_12 = L_11;
		NullCheck(L_12);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_12, _stringLiteral630D82DDB91BD0A38D5F5BE931D8D7433736DCDC, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_13 = L_12;
		NullCheck(L_13);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_13, _stringLiteralA199A05EF4B94C954675DBB8F451DCE18F37C004, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_14 = L_13;
		NullCheck(L_14);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_14, _stringLiteral9C04205D437A60FD17FE5555B3502042C6A436F3, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_15 = L_14;
		NullCheck(L_15);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_15, _stringLiteralF680D63E130D5BE4EB8EAF5C9C69BFD423331BD0, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_16 = L_15;
		NullCheck(L_16);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_16, _stringLiteralD4E298E8840B5A18F01203A40A9ABA9D222F56FE, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_17 = L_16;
		NullCheck(L_17);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_17, _stringLiteralBD6390C3D498E36B80DABABBC961431C400B0719, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_18 = L_17;
		NullCheck(L_18);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_18, _stringLiteral89BF96A86F8214B4D8D2B1FAC0423C4D55046B94, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_19 = L_18;
		NullCheck(L_19);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_19, _stringLiteral73D105A88284D23B2E558768E82867E4EDBEE9C0, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_20 = L_19;
		NullCheck(L_20);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_20, _stringLiteral0DA8B99A80456CF0447EE4E9C8076CC92116C70F, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_21 = L_20;
		NullCheck(L_21);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_21, _stringLiteralECF501476651A1D856C0760EB5C4004165FDCD87, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_22 = L_21;
		NullCheck(L_22);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_22, _stringLiteralEB6A7D6D86E053EAEC72B09FACC475E0F45D4E42, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_23 = L_22;
		NullCheck(L_23);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_23, _stringLiteral89E8BF424EDFA2DED494C2FF771626C991B4968F, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_24 = L_23;
		NullCheck(L_24);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_24, _stringLiteral83A93055434B3DEFBA1FDCD5E3FE2A6CFFCB4D34, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_25 = L_24;
		NullCheck(L_25);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_25, _stringLiteralD3E06A4C4A311F489EB979283DBDC7CBB6CB9CF0, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		__this->___tutorialTextList_13 = L_25;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___tutorialTextList_13), (void*)L_25);
		MonoBehaviour__ctor_m592DB0105CA0BC97AA1C5F4AD27B12D68A3B7C1E(__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Tutorial/<TextAnimation>d__13::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CTextAnimationU3Ed__13__ctor_mDA59BE84AEE7C584D38F61046A3EE671D28D4C4F (U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02* __this, int32_t ___U3CU3E1__state0, const RuntimeMethod* method)
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		int32_t L_0 = ___U3CU3E1__state0;
		__this->___U3CU3E1__state_0 = L_0;
		return;
	}
}
// System.Void Tutorial/<TextAnimation>d__13::System.IDisposable.Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CTextAnimationU3Ed__13_System_IDisposable_Dispose_m44726F4EF16EDC03E203AFE49ED0D8AD0B063257 (U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02* __this, const RuntimeMethod* method)
{
	{
		return;
	}
}
// System.Boolean Tutorial/<TextAnimation>d__13::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CTextAnimationU3Ed__13_MoveNext_m0E2A4D8BBAB10310B55C83F67CDF54C0372B4E9C (U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Dictionary_2_get_Item_mF08AF6105F6A747B98757834E9BE7FE975620925_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_get_Item_m21AEC50E791371101DC22ABCF96A2E46800811F8_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* V_1 = NULL;
	Il2CppChar V_2 = 0x0;
	int32_t V_3 = 0;
	{
		int32_t L_0 = __this->___U3CU3E1__state_0;
		V_0 = L_0;
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_1 = __this->___U3CU3E4__this_2;
		V_1 = L_1;
		int32_t L_2 = V_0;
		if (!L_2)
		{
			goto IL_0017;
		}
	}
	{
		int32_t L_3 = V_0;
		if ((((int32_t)L_3) == ((int32_t)1)))
		{
			goto IL_008c;
		}
	}
	{
		return (bool)0;
	}

IL_0017:
	{
		__this->___U3CU3E1__state_0 = (-1);
		// isAnimating = true;
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_4 = V_1;
		NullCheck(L_4);
		L_4->___isAnimating_10 = (bool)1;
		// for (int i = 0; i < tutorialTextList[index].Length; i++) {
		__this->___U3CiU3E5__2_4 = 0;
		goto IL_00a3;
	}

IL_002e:
	{
		// tutorialText.text += tutorialTextList[index][i];
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_5 = V_1;
		NullCheck(L_5);
		TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* L_6 = L_5->___tutorialText_5;
		TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* L_7 = L_6;
		NullCheck(L_7);
		String_t* L_8;
		L_8 = VirtualFuncInvoker0< String_t* >::Invoke(65 /* System.String TMPro.TMP_Text::get_text() */, L_7);
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_9 = V_1;
		NullCheck(L_9);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_10 = L_9->___tutorialTextList_13;
		int32_t L_11 = __this->___index_3;
		NullCheck(L_10);
		String_t* L_12;
		L_12 = List_1_get_Item_m21AEC50E791371101DC22ABCF96A2E46800811F8(L_10, L_11, List_1_get_Item_m21AEC50E791371101DC22ABCF96A2E46800811F8_RuntimeMethod_var);
		int32_t L_13 = __this->___U3CiU3E5__2_4;
		NullCheck(L_12);
		Il2CppChar L_14;
		L_14 = String_get_Chars_mC49DF0CD2D3BE7BE97B3AD9C995BE3094F8E36D3(L_12, L_13, NULL);
		V_2 = L_14;
		String_t* L_15;
		L_15 = Char_ToString_m2A308731F9577C06AF3C0901234E2EAC8327410C((&V_2), NULL);
		String_t* L_16;
		L_16 = String_Concat_mAF2CE02CC0CB7460753D0A1A91CCF2B1E9804C5D(L_8, L_15, NULL);
		NullCheck(L_7);
		VirtualActionInvoker1< String_t* >::Invoke(66 /* System.Void TMPro.TMP_Text::set_text(System.String) */, L_7, L_16);
		// yield return scripts.delays[0.02f];
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_17 = V_1;
		NullCheck(L_17);
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_18 = L_17->___scripts_8;
		NullCheck(L_18);
		Dictionary_2_t5826E39B66190EFF2D3EA81361D5FC4BB8D6B212* L_19 = L_18->___delays_26;
		NullCheck(L_19);
		WaitForSeconds_tF179DF251655B8DF044952E70A60DF4B358A3DD3* L_20;
		L_20 = Dictionary_2_get_Item_mF08AF6105F6A747B98757834E9BE7FE975620925(L_19, (0.0199999996f), Dictionary_2_get_Item_mF08AF6105F6A747B98757834E9BE7FE975620925_RuntimeMethod_var);
		__this->___U3CU3E2__current_1 = L_20;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CU3E2__current_1), (void*)L_20);
		__this->___U3CU3E1__state_0 = 1;
		return (bool)1;
	}

IL_008c:
	{
		__this->___U3CU3E1__state_0 = (-1);
		// for (int i = 0; i < tutorialTextList[index].Length; i++) {
		int32_t L_21 = __this->___U3CiU3E5__2_4;
		V_3 = L_21;
		int32_t L_22 = V_3;
		__this->___U3CiU3E5__2_4 = ((int32_t)il2cpp_codegen_add((int32_t)L_22, (int32_t)1));
	}

IL_00a3:
	{
		// for (int i = 0; i < tutorialTextList[index].Length; i++) {
		int32_t L_23 = __this->___U3CiU3E5__2_4;
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_24 = V_1;
		NullCheck(L_24);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_25 = L_24->___tutorialTextList_13;
		int32_t L_26 = __this->___index_3;
		NullCheck(L_25);
		String_t* L_27;
		L_27 = List_1_get_Item_m21AEC50E791371101DC22ABCF96A2E46800811F8(L_25, L_26, List_1_get_Item_m21AEC50E791371101DC22ABCF96A2E46800811F8_RuntimeMethod_var);
		NullCheck(L_27);
		int32_t L_28;
		L_28 = String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline(L_27, NULL);
		if ((((int32_t)L_23) < ((int32_t)L_28)))
		{
			goto IL_002e;
		}
	}
	{
		// isAnimating = false;
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_29 = V_1;
		NullCheck(L_29);
		L_29->___isAnimating_10 = (bool)0;
		// if (curIndex == 21) { preventAttack = false; }
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_30 = V_1;
		NullCheck(L_30);
		int32_t L_31 = L_30->___curIndex_7;
		if ((!(((uint32_t)L_31) == ((uint32_t)((int32_t)21)))))
		{
			goto IL_00dc;
		}
	}
	{
		// if (curIndex == 21) { preventAttack = false; }
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_32 = V_1;
		NullCheck(L_32);
		L_32->___preventAttack_9 = (bool)0;
	}

IL_00dc:
	{
		// }
		return (bool)0;
	}
}
// System.Object Tutorial/<TextAnimation>d__13::System.Collections.Generic.IEnumerator<System.Object>.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* U3CTextAnimationU3Ed__13_System_Collections_Generic_IEnumeratorU3CSystem_ObjectU3E_get_Current_m464E3FAD88635390FA64B5C09FDC45BD9FB536ED (U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02* __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = __this->___U3CU3E2__current_1;
		return L_0;
	}
}
// System.Void Tutorial/<TextAnimation>d__13::System.Collections.IEnumerator.Reset()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CTextAnimationU3Ed__13_System_Collections_IEnumerator_Reset_m241E39AA3DC8025128EE588DABBA361C4F5BF389 (U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02* __this, const RuntimeMethod* method)
{
	{
		NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A* L_0 = (NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A_il2cpp_TypeInfo_var)));
		NotSupportedException__ctor_m1398D0CDE19B36AA3DE9392879738C1EA2439CDF(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&U3CTextAnimationU3Ed__13_System_Collections_IEnumerator_Reset_m241E39AA3DC8025128EE588DABBA361C4F5BF389_RuntimeMethod_var)));
	}
}
// System.Object Tutorial/<TextAnimation>d__13::System.Collections.IEnumerator.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* U3CTextAnimationU3Ed__13_System_Collections_IEnumerator_get_Current_m7798178E424523F4C34F54A76DA94FA567530872 (U3CTextAnimationU3Ed__13_t704D9974FEE7DFB8EC401A5752586BD5A28F3A02* __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = __this->___U3CU3E2__current_1;
		return L_0;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Tutorial/<TextAnimation>d__14::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CTextAnimationU3Ed__14__ctor_m5AD9D83A2EB7D2C21A3F746E47C8FE50FBB7AA5A (U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8* __this, int32_t ___U3CU3E1__state0, const RuntimeMethod* method)
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		int32_t L_0 = ___U3CU3E1__state0;
		__this->___U3CU3E1__state_0 = L_0;
		return;
	}
}
// System.Void Tutorial/<TextAnimation>d__14::System.IDisposable.Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CTextAnimationU3Ed__14_System_IDisposable_Dispose_m16C953E0762066125C74CE71F202760A654E9930 (U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8* __this, const RuntimeMethod* method)
{
	{
		return;
	}
}
// System.Boolean Tutorial/<TextAnimation>d__14::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CTextAnimationU3Ed__14_MoveNext_m42259A21C97B479A52350570B8B635FEBC659A3C (U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Dictionary_2_get_Item_mF08AF6105F6A747B98757834E9BE7FE975620925_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* V_1 = NULL;
	Il2CppChar V_2 = 0x0;
	{
		int32_t L_0 = __this->___U3CU3E1__state_0;
		V_0 = L_0;
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_1 = __this->___U3CU3E4__this_2;
		V_1 = L_1;
		int32_t L_2 = V_0;
		if (!L_2)
		{
			goto IL_0017;
		}
	}
	{
		int32_t L_3 = V_0;
		if ((((int32_t)L_3) == ((int32_t)1)))
		{
			goto IL_008d;
		}
	}
	{
		return (bool)0;
	}

IL_0017:
	{
		__this->___U3CU3E1__state_0 = (-1);
		// isAnimating = true;
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_4 = V_1;
		NullCheck(L_4);
		L_4->___isAnimating_10 = (bool)1;
		// foreach (char c in str) {
		String_t* L_5 = __this->___str_3;
		__this->___U3CU3E7__wrap1_4 = L_5;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CU3E7__wrap1_4), (void*)L_5);
		__this->___U3CU3E7__wrap2_5 = 0;
		goto IL_00a2;
	}

IL_003a:
	{
		// foreach (char c in str) {
		String_t* L_6 = __this->___U3CU3E7__wrap1_4;
		int32_t L_7 = __this->___U3CU3E7__wrap2_5;
		NullCheck(L_6);
		Il2CppChar L_8;
		L_8 = String_get_Chars_mC49DF0CD2D3BE7BE97B3AD9C995BE3094F8E36D3(L_6, L_7, NULL);
		V_2 = L_8;
		// statText.text += c;
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_9 = V_1;
		NullCheck(L_9);
		TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* L_10 = L_9->___statText_6;
		TextMeshProUGUI_t101091AF4B578BB534C92E9D1EEAF0611636D957* L_11 = L_10;
		NullCheck(L_11);
		String_t* L_12;
		L_12 = VirtualFuncInvoker0< String_t* >::Invoke(65 /* System.String TMPro.TMP_Text::get_text() */, L_11);
		String_t* L_13;
		L_13 = Char_ToString_m2A308731F9577C06AF3C0901234E2EAC8327410C((&V_2), NULL);
		String_t* L_14;
		L_14 = String_Concat_mAF2CE02CC0CB7460753D0A1A91CCF2B1E9804C5D(L_12, L_13, NULL);
		NullCheck(L_11);
		VirtualActionInvoker1< String_t* >::Invoke(66 /* System.Void TMPro.TMP_Text::set_text(System.String) */, L_11, L_14);
		// yield return scripts.delays[0.02f];
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_15 = V_1;
		NullCheck(L_15);
		Scripts_tA1F67F81394769A4746B08F203BBD942A423AF5C* L_16 = L_15->___scripts_8;
		NullCheck(L_16);
		Dictionary_2_t5826E39B66190EFF2D3EA81361D5FC4BB8D6B212* L_17 = L_16->___delays_26;
		NullCheck(L_17);
		WaitForSeconds_tF179DF251655B8DF044952E70A60DF4B358A3DD3* L_18;
		L_18 = Dictionary_2_get_Item_mF08AF6105F6A747B98757834E9BE7FE975620925(L_17, (0.0199999996f), Dictionary_2_get_Item_mF08AF6105F6A747B98757834E9BE7FE975620925_RuntimeMethod_var);
		__this->___U3CU3E2__current_1 = L_18;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CU3E2__current_1), (void*)L_18);
		__this->___U3CU3E1__state_0 = 1;
		return (bool)1;
	}

IL_008d:
	{
		__this->___U3CU3E1__state_0 = (-1);
		int32_t L_19 = __this->___U3CU3E7__wrap2_5;
		__this->___U3CU3E7__wrap2_5 = ((int32_t)il2cpp_codegen_add((int32_t)L_19, (int32_t)1));
	}

IL_00a2:
	{
		// foreach (char c in str) {
		int32_t L_20 = __this->___U3CU3E7__wrap2_5;
		String_t* L_21 = __this->___U3CU3E7__wrap1_4;
		NullCheck(L_21);
		int32_t L_22;
		L_22 = String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline(L_21, NULL);
		if ((((int32_t)L_20) < ((int32_t)L_22)))
		{
			goto IL_003a;
		}
	}
	{
		__this->___U3CU3E7__wrap1_4 = (String_t*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CU3E7__wrap1_4), (void*)(String_t*)NULL);
		// isAnimating = false;
		Tutorial_t4FF9FF1EF6F8E11558052253ECFBB53EC9AA41D4* L_23 = V_1;
		NullCheck(L_23);
		L_23->___isAnimating_10 = (bool)0;
		// }
		return (bool)0;
	}
}
// System.Object Tutorial/<TextAnimation>d__14::System.Collections.Generic.IEnumerator<System.Object>.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* U3CTextAnimationU3Ed__14_System_Collections_Generic_IEnumeratorU3CSystem_ObjectU3E_get_Current_m084E5B3A8A77E140FD7F4DD10CAC9C22620D3CBB (U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8* __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = __this->___U3CU3E2__current_1;
		return L_0;
	}
}
// System.Void Tutorial/<TextAnimation>d__14::System.Collections.IEnumerator.Reset()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CTextAnimationU3Ed__14_System_Collections_IEnumerator_Reset_mCE1FC3BE7799586C509C11FA2AF2C109B46A280A (U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8* __this, const RuntimeMethod* method)
{
	{
		NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A* L_0 = (NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A_il2cpp_TypeInfo_var)));
		NotSupportedException__ctor_m1398D0CDE19B36AA3DE9392879738C1EA2439CDF(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&U3CTextAnimationU3Ed__14_System_Collections_IEnumerator_Reset_mCE1FC3BE7799586C509C11FA2AF2C109B46A280A_RuntimeMethod_var)));
	}
}
// System.Object Tutorial/<TextAnimation>d__14::System.Collections.IEnumerator.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* U3CTextAnimationU3Ed__14_System_Collections_IEnumerator_get_Current_m4BB5CD1F543B9AC61933FE8BEE9FA54B07D6648E (U3CTextAnimationU3Ed__14_t4EEE013B488311A78C727E35BDF74DC7DCCE6DB8* __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = __this->___U3CU3E2__current_1;
		return L_0;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Fader::OnEnable()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Fader_OnEnable_mD0B5B202A7F14CB1A07AB920A795A3840C51E9D3 (Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Fader_OnLevelFinishedLoading_m9904937D6C2056B6AB61465D48D3818119985E2B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&UnityAction_2__ctor_m0E0C01B7056EB1CB1E6C6F4FC457EBCA3F6B0041_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		// SceneManager.sceneLoaded += OnLevelFinishedLoading;
		UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A* L_0 = (UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A*)il2cpp_codegen_object_new(UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A_il2cpp_TypeInfo_var);
		UnityAction_2__ctor_m0E0C01B7056EB1CB1E6C6F4FC457EBCA3F6B0041(L_0, __this, ((intptr_t)Fader_OnLevelFinishedLoading_m9904937D6C2056B6AB61465D48D3818119985E2B_RuntimeMethod_var), /*hidden argument*/UnityAction_2__ctor_m0E0C01B7056EB1CB1E6C6F4FC457EBCA3F6B0041_RuntimeMethod_var);
		il2cpp_codegen_runtime_class_init_inline(SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var);
		SceneManager_add_sceneLoaded_mDE45940CCEC5D17EB92EB76DB8931E5483FBCD2C(L_0, NULL);
		// }
		return;
	}
}
// System.Void Fader::OnDisable()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Fader_OnDisable_m32A3C918AAD7B8E3C56C15F701DAF1C1052BC634 (Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Fader_OnLevelFinishedLoading_m9904937D6C2056B6AB61465D48D3818119985E2B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&UnityAction_2__ctor_m0E0C01B7056EB1CB1E6C6F4FC457EBCA3F6B0041_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		// SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A* L_0 = (UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A*)il2cpp_codegen_object_new(UnityAction_2_t1C08AEB5AA4F72FEFAB7F303E33C8CFFF80A8C3A_il2cpp_TypeInfo_var);
		UnityAction_2__ctor_m0E0C01B7056EB1CB1E6C6F4FC457EBCA3F6B0041(L_0, __this, ((intptr_t)Fader_OnLevelFinishedLoading_m9904937D6C2056B6AB61465D48D3818119985E2B_RuntimeMethod_var), /*hidden argument*/UnityAction_2__ctor_m0E0C01B7056EB1CB1E6C6F4FC457EBCA3F6B0041_RuntimeMethod_var);
		il2cpp_codegen_runtime_class_init_inline(SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var);
		SceneManager_remove_sceneLoaded_m8840CC33052C4A09A52BF927C3738A7B66783155(L_0, NULL);
		// }
		return;
	}
}
// System.Void Fader::InitiateFader()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Fader_InitiateFader_m07AE039731CCE36D0E46B5BECF23F89D1E6BBF4E (Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_m22ACF33DC0AB281D8B1E18650516D0765006FE66_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_mA3B0428368982ED39ADEBB220EE67D1E99D8B2D2_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponent_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_mE74EE63C85A63FC34DCFC631BC229207B420BC79_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralAB6C48F356D6E8BD57F319FACD990F998C0B878B);
		s_Il2CppMethodInitialized = true;
	}
	{
		// DontDestroyOnLoad(gameObject);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_0;
		L_0 = Component_get_gameObject_m57AEFBB14DB39EC476F740BA000E170355DE691B(__this, NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		Object_DontDestroyOnLoad_m303AA1C4DC810349F285B4809E426CBBA8F834F9(L_0, NULL);
		// if (transform.GetComponent<CanvasGroup>())
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_1;
		L_1 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_1);
		CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094* L_2;
		L_2 = Component_GetComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_mA3B0428368982ED39ADEBB220EE67D1E99D8B2D2(L_1, Component_GetComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_mA3B0428368982ED39ADEBB220EE67D1E99D8B2D2_RuntimeMethod_var);
		bool L_3;
		L_3 = Object_op_Implicit_m18E1885C296CC868AC918101523697CFE6413C79(L_2, NULL);
		if (!L_3)
		{
			goto IL_002e;
		}
	}
	{
		// myCanvas = transform.GetComponent<CanvasGroup>();
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_4;
		L_4 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_4);
		CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094* L_5;
		L_5 = Component_GetComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_mA3B0428368982ED39ADEBB220EE67D1E99D8B2D2(L_4, Component_GetComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_mA3B0428368982ED39ADEBB220EE67D1E99D8B2D2_RuntimeMethod_var);
		__this->___myCanvas_10 = L_5;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___myCanvas_10), (void*)L_5);
	}

IL_002e:
	{
		// if (transform.GetComponentInChildren<Image>())
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_6;
		L_6 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_6);
		Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* L_7;
		L_7 = Component_GetComponentInChildren_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_m22ACF33DC0AB281D8B1E18650516D0765006FE66(L_6, Component_GetComponentInChildren_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_m22ACF33DC0AB281D8B1E18650516D0765006FE66_RuntimeMethod_var);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_8;
		L_8 = Object_op_Implicit_m18E1885C296CC868AC918101523697CFE6413C79(L_7, NULL);
		if (!L_8)
		{
			goto IL_0062;
		}
	}
	{
		// bg = transform.GetComponent<Image>();
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_9;
		L_9 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_9);
		Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* L_10;
		L_10 = Component_GetComponent_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_mE74EE63C85A63FC34DCFC631BC229207B420BC79(L_9, Component_GetComponent_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_mE74EE63C85A63FC34DCFC631BC229207B420BC79_RuntimeMethod_var);
		__this->___bg_11 = L_10;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___bg_11), (void*)L_10);
		// bg.color = fadeColor;
		Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* L_11 = __this->___bg_11;
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_12 = __this->___fadeColor_8;
		NullCheck(L_11);
		VirtualActionInvoker1< Color_tD001788D726C3A7F1379BEED0260B9591F440C1F >::Invoke(23 /* System.Void UnityEngine.UI.Graphic::set_color(UnityEngine.Color) */, L_11, L_12);
	}

IL_0062:
	{
		// if (myCanvas && bg)
		CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094* L_13 = __this->___myCanvas_10;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_14;
		L_14 = Object_op_Implicit_m18E1885C296CC868AC918101523697CFE6413C79(L_13, NULL);
		if (!L_14)
		{
			goto IL_009a;
		}
	}
	{
		Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* L_15 = __this->___bg_11;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_16;
		L_16 = Object_op_Implicit_m18E1885C296CC868AC918101523697CFE6413C79(L_15, NULL);
		if (!L_16)
		{
			goto IL_009a;
		}
	}
	{
		// myCanvas.alpha = 0.0f;
		CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094* L_17 = __this->___myCanvas_10;
		NullCheck(L_17);
		CanvasGroup_set_alpha_m5C06839316D948BB4F75ED72C87FA1F1A20C333F(L_17, (0.0f), NULL);
		// StartCoroutine(FadeIt());
		RuntimeObject* L_18;
		L_18 = Fader_FadeIt_m9A0F19C0561342ECC35B4C63D1C0F6D21263E513(__this, NULL);
		Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* L_19;
		L_19 = MonoBehaviour_StartCoroutine_m4CAFF732AA28CD3BDC5363B44A863575530EC812(__this, L_18, NULL);
		return;
	}

IL_009a:
	{
		// Debug.LogWarning("Something is missing please reimport the package.");
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogWarning_mEF15C6B17CE4E1FA7E379CDB82CE40FCD89A3F28(_stringLiteralAB6C48F356D6E8BD57F319FACD990F998C0B878B, NULL);
		// }
		return;
	}
}
// System.Collections.IEnumerator Fader::FadeIt()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Fader_FadeIt_m9A0F19C0561342ECC35B4C63D1C0F6D21263E513 (Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099* L_0 = (U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099*)il2cpp_codegen_object_new(U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099_il2cpp_TypeInfo_var);
		U3CFadeItU3Ed__13__ctor_m4983806A6FB489F57E177DCD8B170905C370C907(L_0, 0, /*hidden argument*/NULL);
		U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099* L_1 = L_0;
		NullCheck(L_1);
		L_1->___U3CU3E4__this_2 = __this;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___U3CU3E4__this_2), (void*)__this);
		return L_1;
	}
}
// System.Single Fader::newAlpha(System.Single,System.Int32,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Fader_newAlpha_m025F82F0F336A3AC99E1367DF9A1E1B3C02D3642 (Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* __this, float ___delta0, int32_t ___to1, float ___currAlpha2, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___to1;
		if (!L_0)
		{
			goto IL_0009;
		}
	}
	{
		int32_t L_1 = ___to1;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0026;
		}
	}
	{
		goto IL_0041;
	}

IL_0009:
	{
		// currAlpha -= fadeDamp * delta;
		float L_2 = ___currAlpha2;
		float L_3 = __this->___fadeDamp_5;
		float L_4 = ___delta0;
		___currAlpha2 = ((float)il2cpp_codegen_subtract((float)L_2, (float)((float)il2cpp_codegen_multiply((float)L_3, (float)L_4))));
		// if (currAlpha <= 0)
		float L_5 = ___currAlpha2;
		if ((!(((float)L_5) <= ((float)(0.0f)))))
		{
			goto IL_0041;
		}
	}
	{
		// currAlpha = 0;
		___currAlpha2 = (0.0f);
		// break;
		goto IL_0041;
	}

IL_0026:
	{
		// currAlpha += fadeDamp * delta;
		float L_6 = ___currAlpha2;
		float L_7 = __this->___fadeDamp_5;
		float L_8 = ___delta0;
		___currAlpha2 = ((float)il2cpp_codegen_add((float)L_6, (float)((float)il2cpp_codegen_multiply((float)L_7, (float)L_8))));
		// if (currAlpha >= 1)
		float L_9 = ___currAlpha2;
		if ((!(((float)L_9) >= ((float)(1.0f)))))
		{
			goto IL_0041;
		}
	}
	{
		// currAlpha = 1;
		___currAlpha2 = (1.0f);
	}

IL_0041:
	{
		// return currAlpha;
		float L_10 = ___currAlpha2;
		return L_10;
	}
}
// System.Void Fader::OnLevelFinishedLoading(UnityEngine.SceneManagement.Scene,UnityEngine.SceneManagement.LoadSceneMode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Fader_OnLevelFinishedLoading_m9904937D6C2056B6AB61465D48D3818119985E2B (Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* __this, Scene_tA1DC762B79745EB5140F054C884855B922318356 ___scene0, int32_t ___mode1, const RuntimeMethod* method)
{
	{
		// StartCoroutine(FadeIt());
		RuntimeObject* L_0;
		L_0 = Fader_FadeIt_m9A0F19C0561342ECC35B4C63D1C0F6D21263E513(__this, NULL);
		Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* L_1;
		L_1 = MonoBehaviour_StartCoroutine_m4CAFF732AA28CD3BDC5363B44A863575530EC812(__this, L_0, NULL);
		// isFadeIn = true;
		__this->___isFadeIn_9 = (bool)1;
		// }
		return;
	}
}
// System.Void Fader::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Fader__ctor_m827B0782132394F7C49D788A10113C287FC6207A (Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* __this, const RuntimeMethod* method)
{
	{
		MonoBehaviour__ctor_m592DB0105CA0BC97AA1C5F4AD27B12D68A3B7C1E(__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Fader/<FadeIt>d__13::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CFadeItU3Ed__13__ctor_m4983806A6FB489F57E177DCD8B170905C370C907 (U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099* __this, int32_t ___U3CU3E1__state0, const RuntimeMethod* method)
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		int32_t L_0 = ___U3CU3E1__state0;
		__this->___U3CU3E1__state_0 = L_0;
		return;
	}
}
// System.Void Fader/<FadeIt>d__13::System.IDisposable.Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CFadeItU3Ed__13_System_IDisposable_Dispose_m6FE0715F1D569AADE32EAFA00EC29E40B80C1425 (U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099* __this, const RuntimeMethod* method)
{
	{
		return;
	}
}
// System.Boolean Fader/<FadeIt>d__13::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CFadeItU3Ed__13_MoveNext_m2C2D6B869EE998017209FF4F152C1DC1686EAFD4 (U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099* __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* V_1 = NULL;
	float V_2 = 0.0f;
	{
		int32_t L_0 = __this->___U3CU3E1__state_0;
		V_0 = L_0;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_1 = __this->___U3CU3E4__this_2;
		V_1 = L_1;
		int32_t L_2 = V_0;
		switch (L_2)
		{
			case 0:
			{
				goto IL_0026;
			}
			case 1:
			{
				goto IL_003f;
			}
			case 2:
			{
				goto IL_0112;
			}
			case 3:
			{
				goto IL_0144;
			}
		}
	}
	{
		return (bool)0;
	}

IL_0026:
	{
		__this->___U3CU3E1__state_0 = (-1);
		goto IL_0046;
	}

IL_002f:
	{
		// yield return null;
		__this->___U3CU3E2__current_1 = NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CU3E2__current_1), (void*)NULL);
		__this->___U3CU3E1__state_0 = 1;
		return (bool)1;
	}

IL_003f:
	{
		__this->___U3CU3E1__state_0 = (-1);
	}

IL_0046:
	{
		// while (!start)
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_3 = V_1;
		NullCheck(L_3);
		bool L_4 = L_3->___start_4;
		if (!L_4)
		{
			goto IL_002f;
		}
	}
	{
		// lastTime = Time.time;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_5 = V_1;
		float L_6;
		L_6 = Time_get_time_m0BEE9AACD0723FE414465B77C9C64D12263675F3(NULL);
		NullCheck(L_5);
		L_5->___lastTime_12 = L_6;
		// float coDelta = lastTime;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_7 = V_1;
		NullCheck(L_7);
		float L_8 = L_7->___lastTime_12;
		V_2 = L_8;
		// bool hasFadedIn = false;
		__this->___U3ChasFadedInU3E5__2_3 = (bool)0;
		goto IL_0119;
	}

IL_006c:
	{
		// coDelta = Time.time - lastTime;
		float L_9;
		L_9 = Time_get_time_m0BEE9AACD0723FE414465B77C9C64D12263675F3(NULL);
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_10 = V_1;
		NullCheck(L_10);
		float L_11 = L_10->___lastTime_12;
		V_2 = ((float)il2cpp_codegen_subtract((float)L_9, (float)L_11));
		// if (!isFadeIn)
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_12 = V_1;
		NullCheck(L_12);
		bool L_13 = L_12->___isFadeIn_9;
		if (L_13)
		{
			goto IL_00be;
		}
	}
	{
		// alpha = newAlpha(coDelta, 1, alpha);
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_14 = V_1;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_15 = V_1;
		float L_16 = V_2;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_17 = V_1;
		NullCheck(L_17);
		float L_18 = L_17->___alpha_7;
		NullCheck(L_15);
		float L_19;
		L_19 = Fader_newAlpha_m025F82F0F336A3AC99E1367DF9A1E1B3C02D3642(L_15, L_16, 1, L_18, NULL);
		NullCheck(L_14);
		L_14->___alpha_7 = L_19;
		// if (alpha == 1 && !startedLoading)
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_20 = V_1;
		NullCheck(L_20);
		float L_21 = L_20->___alpha_7;
		if ((!(((float)L_21) == ((float)(1.0f)))))
		{
			goto IL_00e6;
		}
	}
	{
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_22 = V_1;
		NullCheck(L_22);
		bool L_23 = L_22->___startedLoading_13;
		if (L_23)
		{
			goto IL_00e6;
		}
	}
	{
		// startedLoading = true;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_24 = V_1;
		NullCheck(L_24);
		L_24->___startedLoading_13 = (bool)1;
		// SceneManager.LoadScene(fadeScene);
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_25 = V_1;
		NullCheck(L_25);
		String_t* L_26 = L_25->___fadeScene_6;
		il2cpp_codegen_runtime_class_init_inline(SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var);
		SceneManager_LoadScene_m7237839058F581BFCA0A79BB96F6F931469E43CF(L_26, NULL);
		goto IL_00e6;
	}

IL_00be:
	{
		// alpha = newAlpha(coDelta, 0, alpha);
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_27 = V_1;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_28 = V_1;
		float L_29 = V_2;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_30 = V_1;
		NullCheck(L_30);
		float L_31 = L_30->___alpha_7;
		NullCheck(L_28);
		float L_32;
		L_32 = Fader_newAlpha_m025F82F0F336A3AC99E1367DF9A1E1B3C02D3642(L_28, L_29, 0, L_31, NULL);
		NullCheck(L_27);
		L_27->___alpha_7 = L_32;
		// if (alpha == 0)
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_33 = V_1;
		NullCheck(L_33);
		float L_34 = L_33->___alpha_7;
		if ((!(((float)L_34) == ((float)(0.0f)))))
		{
			goto IL_00e6;
		}
	}
	{
		// hasFadedIn = true;
		__this->___U3ChasFadedInU3E5__2_3 = (bool)1;
	}

IL_00e6:
	{
		// lastTime = Time.time;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_35 = V_1;
		float L_36;
		L_36 = Time_get_time_m0BEE9AACD0723FE414465B77C9C64D12263675F3(NULL);
		NullCheck(L_35);
		L_35->___lastTime_12 = L_36;
		// myCanvas.alpha = alpha;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_37 = V_1;
		NullCheck(L_37);
		CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094* L_38 = L_37->___myCanvas_10;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_39 = V_1;
		NullCheck(L_39);
		float L_40 = L_39->___alpha_7;
		NullCheck(L_38);
		CanvasGroup_set_alpha_m5C06839316D948BB4F75ED72C87FA1F1A20C333F(L_38, L_40, NULL);
		// yield return null;
		__this->___U3CU3E2__current_1 = NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CU3E2__current_1), (void*)NULL);
		__this->___U3CU3E1__state_0 = 2;
		return (bool)1;
	}

IL_0112:
	{
		__this->___U3CU3E1__state_0 = (-1);
	}

IL_0119:
	{
		// while (!hasFadedIn)
		bool L_41 = __this->___U3ChasFadedInU3E5__2_3;
		if (!L_41)
		{
			goto IL_006c;
		}
	}
	{
		// Initiate.DoneFading();
		Initiate_DoneFading_m4180FA79DDCCB3ADF33825C84055701D68428D17(NULL);
		// Destroy(gameObject);
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_42 = V_1;
		NullCheck(L_42);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_43;
		L_43 = Component_get_gameObject_m57AEFBB14DB39EC476F740BA000E170355DE691B(L_42, NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		Object_Destroy_mFCDAE6333522488F60597AF019EA90BB1207A5AA(L_43, NULL);
		// yield return null;
		__this->___U3CU3E2__current_1 = NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CU3E2__current_1), (void*)NULL);
		__this->___U3CU3E1__state_0 = 3;
		return (bool)1;
	}

IL_0144:
	{
		__this->___U3CU3E1__state_0 = (-1);
		// }
		return (bool)0;
	}
}
// System.Object Fader/<FadeIt>d__13::System.Collections.Generic.IEnumerator<System.Object>.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* U3CFadeItU3Ed__13_System_Collections_Generic_IEnumeratorU3CSystem_ObjectU3E_get_Current_m435F6467EC0099098AF46083679A295B9597E568 (U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099* __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = __this->___U3CU3E2__current_1;
		return L_0;
	}
}
// System.Void Fader/<FadeIt>d__13::System.Collections.IEnumerator.Reset()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CFadeItU3Ed__13_System_Collections_IEnumerator_Reset_m5A4CF16FF8C3EE5FF2A0397BBDE87E85BB948611 (U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099* __this, const RuntimeMethod* method)
{
	{
		NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A* L_0 = (NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A_il2cpp_TypeInfo_var)));
		NotSupportedException__ctor_m1398D0CDE19B36AA3DE9392879738C1EA2439CDF(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&U3CFadeItU3Ed__13_System_Collections_IEnumerator_Reset_m5A4CF16FF8C3EE5FF2A0397BBDE87E85BB948611_RuntimeMethod_var)));
	}
}
// System.Object Fader/<FadeIt>d__13::System.Collections.IEnumerator.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* U3CFadeItU3Ed__13_System_Collections_IEnumerator_get_Current_m59FA2B15979DAE5E53D23BC308D79C2AE9F346A0 (U3CFadeItU3Ed__13_t9A150F0B6003A1C0E5F8F003003F10D150AC0099* __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = __this->___U3CU3E2__current_1;
		return L_0;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Initiate::Fade(System.String,UnityEngine.Color,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Initiate_Fade_mFB5298A6C51A8BDFCF276EA747EF18BD7B92E2D4 (String_t* ___scene0, Color_tD001788D726C3A7F1379BEED0260B9591F440C1F ___col1, float ___multiplier2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_AddComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_m1C004B58918BA839B892637D46D95AF04D69DADA_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_AddComponent_TisCanvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26_m13C85FD585C0679530F8B35D0B39D965702FD0F5_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_AddComponent_TisFader_t4082384C0679E40CABDB8F1A51E0246989537D24_m05AEC75245A2C82F9D47A618CC0DE93E72102C3B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_AddComponent_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_mA327C9E1CA12BC531D587E7567F2067B96E6B6A0_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponent_TisFader_t4082384C0679E40CABDB8F1A51E0246989537D24_mC0661A39B823BACE89B865B139AD471E8E5A3B18_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_t76FEDD663AB33C991A9C9A23129337651094216F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Initiate_t4923CFF950DA8CE1826343BD14BE825EDDD5AE2C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral117FBC2BBE50A7C98480590D08F348D5A9097BBD);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralA5BF6288DB720FF5F6257F5F0961A932300AA7AC);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (areWeFading)
		bool L_0 = ((Initiate_t4923CFF950DA8CE1826343BD14BE825EDDD5AE2C_StaticFields*)il2cpp_codegen_static_fields_for(Initiate_t4923CFF950DA8CE1826343BD14BE825EDDD5AE2C_il2cpp_TypeInfo_var))->___areWeFading_0;
		if (!L_0)
		{
			goto IL_0012;
		}
	}
	{
		// Debug.Log("Already Fading");
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_Log_m86567BCF22BBE7809747817453CACA0E41E68219(_stringLiteralA5BF6288DB720FF5F6257F5F0961A932300AA7AC, NULL);
		// return;
		return;
	}

IL_0012:
	{
		// GameObject init = new GameObject();
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_1 = (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)il2cpp_codegen_object_new(GameObject_t76FEDD663AB33C991A9C9A23129337651094216F_il2cpp_TypeInfo_var);
		GameObject__ctor_m7D0340DE160786E6EFA8DABD39EC3B694DA30AAD(L_1, /*hidden argument*/NULL);
		// init.name = "Fader";
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_2 = L_1;
		NullCheck(L_2);
		Object_set_name_mC79E6DC8FFD72479C90F0C4CC7F42A0FEAF5AE47(L_2, _stringLiteral117FBC2BBE50A7C98480590D08F348D5A9097BBD, NULL);
		// Canvas myCanvas = init.AddComponent<Canvas>();
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_3 = L_2;
		NullCheck(L_3);
		Canvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26* L_4;
		L_4 = GameObject_AddComponent_TisCanvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26_m13C85FD585C0679530F8B35D0B39D965702FD0F5(L_3, GameObject_AddComponent_TisCanvas_t2DB4CEFDFF732884866C83F11ABF75F5AE8FFB26_m13C85FD585C0679530F8B35D0B39D965702FD0F5_RuntimeMethod_var);
		// myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		NullCheck(L_4);
		Canvas_set_renderMode_mD73E953F8A115CF469508448A00D0EDAFAF5AB47(L_4, 0, NULL);
		// init.AddComponent<Fader>();
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_5 = L_3;
		NullCheck(L_5);
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_6;
		L_6 = GameObject_AddComponent_TisFader_t4082384C0679E40CABDB8F1A51E0246989537D24_m05AEC75245A2C82F9D47A618CC0DE93E72102C3B(L_5, GameObject_AddComponent_TisFader_t4082384C0679E40CABDB8F1A51E0246989537D24_m05AEC75245A2C82F9D47A618CC0DE93E72102C3B_RuntimeMethod_var);
		// init.AddComponent<CanvasGroup>();
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_7 = L_5;
		NullCheck(L_7);
		CanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094* L_8;
		L_8 = GameObject_AddComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_m1C004B58918BA839B892637D46D95AF04D69DADA(L_7, GameObject_AddComponent_TisCanvasGroup_t048C1461B14628CFAEBE6E7353093ADB04EBC094_m1C004B58918BA839B892637D46D95AF04D69DADA_RuntimeMethod_var);
		// init.AddComponent<Image>();
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_9 = L_7;
		NullCheck(L_9);
		Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* L_10;
		L_10 = GameObject_AddComponent_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_mA327C9E1CA12BC531D587E7567F2067B96E6B6A0(L_9, GameObject_AddComponent_TisImage_tBC1D03F63BF71132E9A5E472B8742F172A011E7E_mA327C9E1CA12BC531D587E7567F2067B96E6B6A0_RuntimeMethod_var);
		// Fader scr = init.GetComponent<Fader>();
		NullCheck(L_9);
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_11;
		L_11 = GameObject_GetComponent_TisFader_t4082384C0679E40CABDB8F1A51E0246989537D24_mC0661A39B823BACE89B865B139AD471E8E5A3B18(L_9, GameObject_GetComponent_TisFader_t4082384C0679E40CABDB8F1A51E0246989537D24_mC0661A39B823BACE89B865B139AD471E8E5A3B18_RuntimeMethod_var);
		// scr.fadeDamp = multiplier;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_12 = L_11;
		float L_13 = ___multiplier2;
		NullCheck(L_12);
		L_12->___fadeDamp_5 = L_13;
		// scr.fadeScene = scene;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_14 = L_12;
		String_t* L_15 = ___scene0;
		NullCheck(L_14);
		L_14->___fadeScene_6 = L_15;
		Il2CppCodeGenWriteBarrier((void**)(&L_14->___fadeScene_6), (void*)L_15);
		// scr.fadeColor = col;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_16 = L_14;
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_17 = ___col1;
		NullCheck(L_16);
		L_16->___fadeColor_8 = L_17;
		// scr.start = true;
		Fader_t4082384C0679E40CABDB8F1A51E0246989537D24* L_18 = L_16;
		NullCheck(L_18);
		L_18->___start_4 = (bool)1;
		// areWeFading = true;
		((Initiate_t4923CFF950DA8CE1826343BD14BE825EDDD5AE2C_StaticFields*)il2cpp_codegen_static_fields_for(Initiate_t4923CFF950DA8CE1826343BD14BE825EDDD5AE2C_il2cpp_TypeInfo_var))->___areWeFading_0 = (bool)1;
		// scr.InitiateFader();
		NullCheck(L_18);
		Fader_InitiateFader_m07AE039731CCE36D0E46B5BECF23F89D1E6BBF4E(L_18, NULL);
		// }
		return;
	}
}
// System.Void Initiate::DoneFading()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Initiate_DoneFading_m4180FA79DDCCB3ADF33825C84055701D68428D17 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Initiate_t4923CFF950DA8CE1826343BD14BE825EDDD5AE2C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		// areWeFading = false;
		((Initiate_t4923CFF950DA8CE1826343BD14BE825EDDD5AE2C_StaticFields*)il2cpp_codegen_static_fields_for(Initiate_t4923CFF950DA8CE1826343BD14BE825EDDD5AE2C_il2cpp_TypeInfo_var))->___areWeFading_0 = (bool)0;
		// }
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.UInt32 <PrivateImplementationDetails>::ComputeStringHash(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t U3CPrivateImplementationDetailsU3E_ComputeStringHash_m9E43873DE0DF480D27EC1C2AA3C09E74EA77F75D (String_t* ___s0, const RuntimeMethod* method)
{
	uint32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		String_t* L_0 = ___s0;
		if (!L_0)
		{
			goto IL_002a;
		}
	}
	{
		V_0 = ((int32_t)-2128831035);
		V_1 = 0;
		goto IL_0021;
	}

IL_000d:
	{
		String_t* L_1 = ___s0;
		int32_t L_2 = V_1;
		NullCheck(L_1);
		Il2CppChar L_3;
		L_3 = String_get_Chars_mC49DF0CD2D3BE7BE97B3AD9C995BE3094F8E36D3(L_1, L_2, NULL);
		uint32_t L_4 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_multiply((int32_t)((int32_t)((int32_t)L_3^(int32_t)L_4)), (int32_t)((int32_t)16777619)));
		int32_t L_5 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1));
	}

IL_0021:
	{
		int32_t L_6 = V_1;
		String_t* L_7 = ___s0;
		NullCheck(L_7);
		int32_t L_8;
		L_8 = String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline(L_7, NULL);
		if ((((int32_t)L_6) < ((int32_t)L_8)))
		{
			goto IL_000d;
		}
	}

IL_002a:
	{
		uint32_t L_9 = V_0;
		return L_9;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Vector2__ctor_m9525B79969AFFE3254B303A40997A56DEEB6F548_inline (Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7* __this, float ___x0, float ___y1, const RuntimeMethod* method)
{
	{
		float L_0 = ___x0;
		__this->___x_0 = L_0;
		float L_1 = ___y1;
		__this->___y_1 = L_1;
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector2_op_Implicit_mCD214B04BC52AED3C89C3BEF664B6247E5F8954A_inline (Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 ___v0, const RuntimeMethod* method)
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 L_0 = ___v0;
		float L_1 = L_0.___x_0;
		Vector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7 L_2 = ___v0;
		float L_3 = L_2.___y_1;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_4;
		memset((&L_4), 0, sizeof(L_4));
		Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline((&L_4), L_1, L_3, (0.0f), /*hidden argument*/NULL);
		V_0 = L_4;
		goto IL_001a;
	}

IL_001a:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_5 = V_0;
		return L_5;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline (String_t* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->____stringLength_4;
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Enumerator_get_Current_m8B42D4B2DE853B9D11B997120CD0228D4780E394_gshared_inline (Enumerator_tF5AC6CD19D283FBD724440520CEE68FE2602F7AF* __this, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType**/Il2CppFullySharedGenericAny* il2cppRetVal, const RuntimeMethod* method)
{
	// sizeof(T)
	const uint32_t SizeOf_T_t010616E3077234188F9BB4FAF369F8571BC5F2E1 = il2cpp_codegen_sizeof(il2cpp_rgctx_data(InitializedTypeInfo(method->klass)->rgctx_data, 2));
	// T
	const Il2CppFullySharedGenericAny L_0 = alloca(SizeOf_T_t010616E3077234188F9BB4FAF369F8571BC5F2E1);
	{
		il2cpp_codegen_memcpy(L_0, il2cpp_codegen_get_instance_field_data_pointer(__this, il2cpp_rgctx_field(il2cpp_rgctx_data(InitializedTypeInfo(method->klass)->rgctx_data, 1),3)), SizeOf_T_t010616E3077234188F9BB4FAF369F8571BC5F2E1);
		il2cpp_codegen_memcpy(il2cppRetVal, L_0, SizeOf_T_t010616E3077234188F9BB4FAF369F8571BC5F2E1);
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t List_1_get_Count_mD2ED26ACAF3BAF386FFEA83893BA51DB9FD8BA30_gshared_inline (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->____size_2;
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void List_1_Add_mD4F3498FBD3BDD3F03CBCFB38041CBAC9C28CAFC_gshared_inline (List_1_tDBA89B0E21BAC58CFBD3C1F76E4668E3B562761A* __this, /*Unity.IL2CPP.Metadata.__Il2CppFullySharedGenericType*/Il2CppFullySharedGenericAny ___item0, const RuntimeMethod* method)
{
	// sizeof(T)
	const uint32_t SizeOf_T_t664E2061A913AF1FEE499655BC64F0FDE10D2A5E = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->klass->rgctx_data, 9));
	// T
	const Il2CppFullySharedGenericAny L_8 = alloca(SizeOf_T_t664E2061A913AF1FEE499655BC64F0FDE10D2A5E);
	const Il2CppFullySharedGenericAny L_9 = L_8;
	__Il2CppFullySharedGenericTypeU5BU5D_tCAB6D060972DD49223A834B7EEFEB9FE2D003BEC* V_0 = NULL;
	int32_t V_1 = 0;
	{
		int32_t L_0 = (int32_t)__this->____version_3;
		__this->____version_3 = ((int32_t)il2cpp_codegen_add((int32_t)L_0, (int32_t)1));
		__Il2CppFullySharedGenericTypeU5BU5D_tCAB6D060972DD49223A834B7EEFEB9FE2D003BEC* L_1 = (__Il2CppFullySharedGenericTypeU5BU5D_tCAB6D060972DD49223A834B7EEFEB9FE2D003BEC*)__this->____items_1;
		V_0 = L_1;
		int32_t L_2 = (int32_t)__this->____size_2;
		V_1 = L_2;
		int32_t L_3 = V_1;
		__Il2CppFullySharedGenericTypeU5BU5D_tCAB6D060972DD49223A834B7EEFEB9FE2D003BEC* L_4 = V_0;
		NullCheck(L_4);
		if ((!(((uint32_t)L_3) < ((uint32_t)((int32_t)((int32_t)(((RuntimeArray*)L_4)->max_length)))))))
		{
			goto IL_0034;
		}
	}
	{
		int32_t L_5 = V_1;
		__this->____size_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1));
		__Il2CppFullySharedGenericTypeU5BU5D_tCAB6D060972DD49223A834B7EEFEB9FE2D003BEC* L_6 = V_0;
		int32_t L_7 = V_1;
		il2cpp_codegen_memcpy(L_8, (il2cpp_codegen_class_is_value_type(il2cpp_rgctx_data(method->klass->rgctx_data, 9)) ? ___item0 : &___item0), SizeOf_T_t664E2061A913AF1FEE499655BC64F0FDE10D2A5E);
		NullCheck(L_6);
		il2cpp_codegen_memcpy((L_6)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_7)), L_8, SizeOf_T_t664E2061A913AF1FEE499655BC64F0FDE10D2A5E);
		Il2CppCodeGenWriteBarrierForClass(il2cpp_rgctx_data(method->klass->rgctx_data, 9), (void**)(L_6)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_7)), (void*)L_8);
		return;
	}

IL_0034:
	{
		il2cpp_codegen_memcpy(L_9, (il2cpp_codegen_class_is_value_type(il2cpp_rgctx_data(method->klass->rgctx_data, 9)) ? ___item0 : &___item0), SizeOf_T_t664E2061A913AF1FEE499655BC64F0FDE10D2A5E);
		InvokerCallActionInvoker1< Il2CppFullySharedGenericAny >::Invoke(il2cpp_rgctx_method(method->klass->rgctx_data, 14), __this, (il2cpp_codegen_class_is_value_type(il2cpp_rgctx_data(method->klass->rgctx_data, 9)) ? L_9: *(void**)L_9));
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2* __this, float ___x0, float ___y1, float ___z2, const RuntimeMethod* method)
{
	{
		float L_0 = ___x0;
		__this->___x_2 = L_0;
		float L_1 = ___y1;
		__this->___y_3 = L_1;
		float L_2 = ___z2;
		__this->___z_4 = L_2;
		return;
	}
}
