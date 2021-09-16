#include "pch-cpp.hpp"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <limits>
#include <stdint.h>


struct InterfaceActionInvoker0
{
	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		invokeData.method->invoker_method(invokeData.methodPtr, invokeData.method, obj, NULL, NULL);
	}
};
template <typename R>
struct InterfaceFuncInvoker0
{
	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		R ret;
		invokeData.method->invoker_method(invokeData.methodPtr, invokeData.method, obj, NULL, &ret);
		return ret;
	}
};

// System.Collections.Generic.IEnumerable`1<System.Int32>
struct IEnumerable_1_tCE758D940790D6D0D56B457E522C195F8C413AF2;
// System.Collections.Generic.IEnumerable`1<System.Single>
struct IEnumerable_1_t352FDDEA001ABE8E1D67849D2E2F3D1D75B03D41;
// System.IntPtr[]
struct IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832;
// System.Diagnostics.StackTrace[]
struct StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF;
// System.ArgumentNullException
struct ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129;
// System.Exception
struct Exception_t;
// System.Collections.IDictionary
struct IDictionary_t6D03155AF1FA9083817AA5B6AD7DEEACC26AB220;
// System.InvalidOperationException
struct InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB;
// System.NotSupportedException
struct NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6;
// System.String
struct String_t;
// System.Void
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915;

IL2CPP_EXTERN_C RuntimeClass* ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* BitConverter_t6E99605185963BC12B3D369E13F2B88997E64A27_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerable_1_t352FDDEA001ABE8E1D67849D2E2F3D1D75B03D41_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerable_1_tCE758D940790D6D0D56B457E522C195F8C413AF2_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerator_1_t736E9F8BD2FD38A5E9EA2E8A510AFED788D05010_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerator_1_tD6A90A7446DA8E6CF865EDFBBF18C1200BB6D452_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral66F9618FDA792CAB23AF2D7FFB50AB2D3E393DC5;
IL2CPP_EXTERN_C String_t* _stringLiteral9D0E978C2541B8A36DFB07E397656689CE9E713F;
IL2CPP_EXTERN_C String_t* _stringLiteralB7E78BE66617B9AE36B6A6E170E3545EE25C1D11;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerable_Max_m30DE6D3273F1E770CC99EC43653F8F4BA4A94760_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerable_Max_mA30ECB22B118A464652A20E12E0097D8A952531D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerable_Min_m08CFCF12550397A30F094F24E2F68AD21E98EC9F_RuntimeMethod_var;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;


IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct U3CModuleU3E_tA3942657997767549ED3B944EB77AFA183BBF4B9 
{
};
struct Il2CppArrayBounds;

// System.Linq.Enumerable
struct Enumerable_t372195206D92B3F390693F9449282C31FD564C09  : public RuntimeObject
{
};

// System.Collections.Generic.EnumerableHelpers
struct EnumerableHelpers_t1F4A45E80739172920C4980F48BB857C36AEF58B  : public RuntimeObject
{
};

// System.Linq.Error
struct Error_tCE0C9D928B2D2CC69DDEC1A0ECF05131938959DB  : public RuntimeObject
{
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

// System.Linq.Utilities
struct Utilities_t5590A80A1C1E67EF8817B3C789BEF9F1E63DE2F0  : public RuntimeObject
{
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

// System.Collections.Generic.CopyPosition
struct CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C 
{
	// System.Int32 System.Collections.Generic.CopyPosition::<Row>k__BackingField
	int32_t ___U3CRowU3Ek__BackingField_0;
	// System.Int32 System.Collections.Generic.CopyPosition::<Column>k__BackingField
	int32_t ___U3CColumnU3Ek__BackingField_1;
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

// System.Collections.Generic.Marker
struct Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5 
{
	// System.Int32 System.Collections.Generic.Marker::<Count>k__BackingField
	int32_t ___U3CCountU3Ek__BackingField_0;
	// System.Int32 System.Collections.Generic.Marker::<Index>k__BackingField
	int32_t ___U3CIndexU3Ek__BackingField_1;
};

// System.Single
struct Single_t4530F2FF86FCB0DC29F35385CA1BD21BE294761C 
{
	// System.Single System.Single::m_value
	float ___m_value_0;
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

// System.SystemException
struct SystemException_tCC48D868298F4C0705279823E34B00F4FBDB7295  : public Exception_t
{
};

// System.ArgumentException
struct ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263  : public SystemException_tCC48D868298F4C0705279823E34B00F4FBDB7295
{
	// System.String System.ArgumentException::_paramName
	String_t* ____paramName_18;
};

// System.InvalidOperationException
struct InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB  : public SystemException_tCC48D868298F4C0705279823E34B00F4FBDB7295
{
};

// System.NotSupportedException
struct NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A  : public SystemException_tCC48D868298F4C0705279823E34B00F4FBDB7295
{
};

// System.ArgumentNullException
struct ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129  : public ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263
{
};
#ifdef __clang__
#pragma clang diagnostic pop
#endif



// System.Void System.ArgumentNullException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ArgumentNullException__ctor_m444AE141157E333844FC1A9500224C2F9FD24F4B (ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129* __this, String_t* ___paramName0, const RuntimeMethod* method);
// System.Void System.InvalidOperationException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162 (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* __this, String_t* ___message0, const RuntimeMethod* method);
// System.Void System.NotSupportedException::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NotSupportedException__ctor_m1398D0CDE19B36AA3DE9392879738C1EA2439CDF (NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A* __this, const RuntimeMethod* method);
// System.Exception System.Linq.Error::ArgumentNull(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Exception_t* Error_ArgumentNull_m9157523765DB73FC9F7B984F2F740F2B5EDB7337 (String_t* ___s0, const RuntimeMethod* method);
// System.Exception System.Linq.Error::NoElements()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Exception_t* Error_NoElements_m0E3D3D6CC1E9C9ED28CD989485766C91DC71042F (const RuntimeMethod* method);
// System.Boolean System.Single::IsNaN(System.Single)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool Single_IsNaN_m684B090AA2F895FD91821CA8684CBC11D784E4DD_inline (float ___f0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.CopyPosition::.ctor(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CopyPosition__ctor_mFDF577B19459EB1D3EB4F7AA29125C2EB8E3604D (CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* __this, int32_t ___row0, int32_t ___column1, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.CopyPosition::get_Row()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t CopyPosition_get_Row_m9D2FDA30FD2F0235CCF3F82EC147B64B8BF44D18_inline (CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* __this, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.CopyPosition::get_Column()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t CopyPosition_get_Column_m217F5811015F2AF2D1381AE8EF5127A2245B6C1E_inline (CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* __this, const RuntimeMethod* method);
// System.Collections.Generic.CopyPosition System.Collections.Generic.CopyPosition::Normalize(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C CopyPosition_Normalize_mAB50E4279BC0C6EC003BE90A65EB38521D0CC164 (CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* __this, int32_t ___endColumn0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Marker::.ctor(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Marker__ctor_m5A9F92F3092A06075D55D96F770EFAC2EFC2450A (Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5* __this, int32_t ___count0, int32_t ___index1, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.Marker::get_Count()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t Marker_get_Count_m7EA2D8F123074BC865257FB803537912244BE94E_inline (Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5* __this, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.Marker::get_Index()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t Marker_get_Index_mB6E75E1F49ADC7E07FDF4B36029802024A549BD0_inline (Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5* __this, const RuntimeMethod* method);
// System.Int32 System.BitConverter::SingleToInt32Bits(System.Single)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t BitConverter_SingleToInt32Bits_mA1902D40966CA4C89A8974B10E5680A06E88566B_inline (float ___value0, const RuntimeMethod* method);
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
// System.Exception System.Linq.Error::ArgumentNull(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Exception_t* Error_ArgumentNull_m9157523765DB73FC9F7B984F2F740F2B5EDB7337 (String_t* ___s0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		String_t* L_0 = ___s0;
		ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129* L_1 = (ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129*)il2cpp_codegen_object_new(ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129_il2cpp_TypeInfo_var);
		ArgumentNullException__ctor_m444AE141157E333844FC1A9500224C2F9FD24F4B(L_1, L_0, /*hidden argument*/NULL);
		return L_1;
	}
}
// System.Exception System.Linq.Error::MoreThanOneMatch()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Exception_t* Error_MoreThanOneMatch_mADF388C1E5EACA4BA8E0CDAAA0834C595544BFAF (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral9D0E978C2541B8A36DFB07E397656689CE9E713F);
		s_Il2CppMethodInitialized = true;
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_0 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var);
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_0, _stringLiteral9D0E978C2541B8A36DFB07E397656689CE9E713F, /*hidden argument*/NULL);
		return L_0;
	}
}
// System.Exception System.Linq.Error::NoElements()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Exception_t* Error_NoElements_m0E3D3D6CC1E9C9ED28CD989485766C91DC71042F (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralB7E78BE66617B9AE36B6A6E170E3545EE25C1D11);
		s_Il2CppMethodInitialized = true;
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_0 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var);
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_0, _stringLiteralB7E78BE66617B9AE36B6A6E170E3545EE25C1D11, /*hidden argument*/NULL);
		return L_0;
	}
}
// System.Exception System.Linq.Error::NotSupported()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Exception_t* Error_NotSupported_mCF634C93975CEC340B837E3A9020AFB0C9D2A522 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A* L_0 = (NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A*)il2cpp_codegen_object_new(NotSupportedException_t1429765983D409BD2986508963C98D214E4EBF4A_il2cpp_TypeInfo_var);
		NotSupportedException__ctor_m1398D0CDE19B36AA3DE9392879738C1EA2439CDF(L_0, /*hidden argument*/NULL);
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
// System.Int32 System.Linq.Enumerable::Max(System.Collections.Generic.IEnumerable`1<System.Int32>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Enumerable_Max_m30DE6D3273F1E770CC99EC43653F8F4BA4A94760 (RuntimeObject* ___source0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerable_1_tCE758D940790D6D0D56B457E522C195F8C413AF2_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerator_1_tD6A90A7446DA8E6CF865EDFBBF18C1200BB6D452_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	RuntimeObject* V_1 = NULL;
	int32_t V_2 = 0;
	Exception_t* __last_unhandled_exception = 0;
	il2cpp::utils::ExceptionSupportStack<int32_t, 1> __leave_targets;
	{
		RuntimeObject* L_0 = ___source0;
		if (L_0)
		{
			goto IL_000e;
		}
	}
	{
		Exception_t* L_1;
		L_1 = Error_ArgumentNull_m9157523765DB73FC9F7B984F2F740F2B5EDB7337(((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral66F9618FDA792CAB23AF2D7FFB50AB2D3E393DC5)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Enumerable_Max_m30DE6D3273F1E770CC99EC43653F8F4BA4A94760_RuntimeMethod_var)));
	}

IL_000e:
	{
		RuntimeObject* L_2 = ___source0;
		NullCheck(L_2);
		RuntimeObject* L_3;
		L_3 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<T> System.Collections.Generic.IEnumerable`1<System.Int32>::GetEnumerator() */, IEnumerable_1_tCE758D940790D6D0D56B457E522C195F8C413AF2_il2cpp_TypeInfo_var, L_2);
		V_1 = L_3;
	}

IL_0015:
	try
	{// begin try (depth: 1)
		{
			RuntimeObject* L_4 = V_1;
			NullCheck(L_4);
			bool L_5;
			L_5 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_4);
			if (L_5)
			{
				goto IL_0023;
			}
		}

IL_001d:
		{
			Exception_t* L_6;
			L_6 = Error_NoElements_m0E3D3D6CC1E9C9ED28CD989485766C91DC71042F(NULL);
			IL2CPP_RAISE_MANAGED_EXCEPTION(L_6, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Enumerable_Max_m30DE6D3273F1E770CC99EC43653F8F4BA4A94760_RuntimeMethod_var)));
		}

IL_0023:
		{
			RuntimeObject* L_7 = V_1;
			NullCheck(L_7);
			int32_t L_8;
			L_8 = InterfaceFuncInvoker0< int32_t >::Invoke(0 /* T System.Collections.Generic.IEnumerator`1<System.Int32>::get_Current() */, IEnumerator_1_tD6A90A7446DA8E6CF865EDFBBF18C1200BB6D452_il2cpp_TypeInfo_var, L_7);
			V_0 = L_8;
			goto IL_0039;
		}

IL_002c:
		{
			RuntimeObject* L_9 = V_1;
			NullCheck(L_9);
			int32_t L_10;
			L_10 = InterfaceFuncInvoker0< int32_t >::Invoke(0 /* T System.Collections.Generic.IEnumerator`1<System.Int32>::get_Current() */, IEnumerator_1_tD6A90A7446DA8E6CF865EDFBBF18C1200BB6D452_il2cpp_TypeInfo_var, L_9);
			V_2 = L_10;
			int32_t L_11 = V_2;
			int32_t L_12 = V_0;
			if ((((int32_t)L_11) <= ((int32_t)L_12)))
			{
				goto IL_0039;
			}
		}

IL_0037:
		{
			int32_t L_13 = V_2;
			V_0 = L_13;
		}

IL_0039:
		{
			RuntimeObject* L_14 = V_1;
			NullCheck(L_14);
			bool L_15;
			L_15 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_14);
			if (L_15)
			{
				goto IL_002c;
			}
		}

IL_0041:
		{
			IL2CPP_LEAVE(0x77, FINALLY_0043);
		}
	}// end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t*)e.ex;
		goto FINALLY_0043;
	}

FINALLY_0043:
	{// begin finally (depth: 1)
		{
			RuntimeObject* L_16 = V_1;
			if (!L_16)
			{
				goto IL_004c;
			}
		}

IL_0046:
		{
			RuntimeObject* L_17 = V_1;
			NullCheck(L_17);
			InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_17);
		}

IL_004c:
		{
			IL2CPP_END_FINALLY(67)
		}
	}// end finally (depth: 1)
	IL2CPP_CLEANUP(67)
	{
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t*)
		IL2CPP_JUMP_TBL(0x77, IL_004d)
	}

IL_004d:
	{
		int32_t L_18 = V_0;
		return L_18;
	}
}
// System.Single System.Linq.Enumerable::Max(System.Collections.Generic.IEnumerable`1<System.Single>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Enumerable_Max_mA30ECB22B118A464652A20E12E0097D8A952531D (RuntimeObject* ___source0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerable_1_t352FDDEA001ABE8E1D67849D2E2F3D1D75B03D41_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerator_1_t736E9F8BD2FD38A5E9EA2E8A510AFED788D05010_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	float V_0 = 0.0f;
	RuntimeObject* V_1 = NULL;
	float V_2 = 0.0f;
	float V_3 = 0.0f;
	Exception_t* __last_unhandled_exception = 0;
	il2cpp::utils::ExceptionSupportStack<int32_t, 2> __leave_targets;
	{
		RuntimeObject* L_0 = ___source0;
		if (L_0)
		{
			goto IL_000e;
		}
	}
	{
		Exception_t* L_1;
		L_1 = Error_ArgumentNull_m9157523765DB73FC9F7B984F2F740F2B5EDB7337(((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral66F9618FDA792CAB23AF2D7FFB50AB2D3E393DC5)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Enumerable_Max_mA30ECB22B118A464652A20E12E0097D8A952531D_RuntimeMethod_var)));
	}

IL_000e:
	{
		RuntimeObject* L_2 = ___source0;
		NullCheck(L_2);
		RuntimeObject* L_3;
		L_3 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<T> System.Collections.Generic.IEnumerable`1<System.Single>::GetEnumerator() */, IEnumerable_1_t352FDDEA001ABE8E1D67849D2E2F3D1D75B03D41_il2cpp_TypeInfo_var, L_2);
		V_1 = L_3;
	}

IL_0015:
	try
	{// begin try (depth: 1)
		{
			RuntimeObject* L_4 = V_1;
			NullCheck(L_4);
			bool L_5;
			L_5 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_4);
			if (L_5)
			{
				goto IL_0023;
			}
		}

IL_001d:
		{
			Exception_t* L_6;
			L_6 = Error_NoElements_m0E3D3D6CC1E9C9ED28CD989485766C91DC71042F(NULL);
			IL2CPP_RAISE_MANAGED_EXCEPTION(L_6, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Enumerable_Max_mA30ECB22B118A464652A20E12E0097D8A952531D_RuntimeMethod_var)));
		}

IL_0023:
		{
			RuntimeObject* L_7 = V_1;
			NullCheck(L_7);
			float L_8;
			L_8 = InterfaceFuncInvoker0< float >::Invoke(0 /* T System.Collections.Generic.IEnumerator`1<System.Single>::get_Current() */, IEnumerator_1_t736E9F8BD2FD38A5E9EA2E8A510AFED788D05010_il2cpp_TypeInfo_var, L_7);
			V_0 = L_8;
			goto IL_003f;
		}

IL_002c:
		{
			RuntimeObject* L_9 = V_1;
			NullCheck(L_9);
			bool L_10;
			L_10 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_9);
			if (L_10)
			{
				goto IL_0038;
			}
		}

IL_0034:
		{
			float L_11 = V_0;
			V_2 = L_11;
			IL2CPP_LEAVE(0x108, FINALLY_0060);
		}

IL_0038:
		{
			RuntimeObject* L_12 = V_1;
			NullCheck(L_12);
			float L_13;
			L_13 = InterfaceFuncInvoker0< float >::Invoke(0 /* T System.Collections.Generic.IEnumerator`1<System.Single>::get_Current() */, IEnumerator_1_t736E9F8BD2FD38A5E9EA2E8A510AFED788D05010_il2cpp_TypeInfo_var, L_12);
			V_0 = L_13;
		}

IL_003f:
		{
			float L_14 = V_0;
			bool L_15;
			L_15 = Single_IsNaN_m684B090AA2F895FD91821CA8684CBC11D784E4DD_inline(L_14, NULL);
			if (L_15)
			{
				goto IL_002c;
			}
		}

IL_0047:
		{
			goto IL_0056;
		}

IL_0049:
		{
			RuntimeObject* L_16 = V_1;
			NullCheck(L_16);
			float L_17;
			L_17 = InterfaceFuncInvoker0< float >::Invoke(0 /* T System.Collections.Generic.IEnumerator`1<System.Single>::get_Current() */, IEnumerator_1_t736E9F8BD2FD38A5E9EA2E8A510AFED788D05010_il2cpp_TypeInfo_var, L_16);
			V_3 = L_17;
			float L_18 = V_3;
			float L_19 = V_0;
			if ((!(((float)L_18) > ((float)L_19))))
			{
				goto IL_0056;
			}
		}

IL_0054:
		{
			float L_20 = V_3;
			V_0 = L_20;
		}

IL_0056:
		{
			RuntimeObject* L_21 = V_1;
			NullCheck(L_21);
			bool L_22;
			L_22 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_21);
			if (L_22)
			{
				goto IL_0049;
			}
		}

IL_005e:
		{
			IL2CPP_LEAVE(0x106, FINALLY_0060);
		}
	}// end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t*)e.ex;
		goto FINALLY_0060;
	}

FINALLY_0060:
	{// begin finally (depth: 1)
		{
			RuntimeObject* L_23 = V_1;
			if (!L_23)
			{
				goto IL_0069;
			}
		}

IL_0063:
		{
			RuntimeObject* L_24 = V_1;
			NullCheck(L_24);
			InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_24);
		}

IL_0069:
		{
			IL2CPP_END_FINALLY(96)
		}
	}// end finally (depth: 1)
	IL2CPP_CLEANUP(96)
	{
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t*)
		IL2CPP_JUMP_TBL(0x108, IL_006c)
		IL2CPP_JUMP_TBL(0x106, IL_006a)
	}

IL_006a:
	{
		float L_25 = V_0;
		return L_25;
	}

IL_006c:
	{
		float L_26 = V_2;
		return L_26;
	}
}
// System.Int32 System.Linq.Enumerable::Min(System.Collections.Generic.IEnumerable`1<System.Int32>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Enumerable_Min_m08CFCF12550397A30F094F24E2F68AD21E98EC9F (RuntimeObject* ___source0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerable_1_tCE758D940790D6D0D56B457E522C195F8C413AF2_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerator_1_tD6A90A7446DA8E6CF865EDFBBF18C1200BB6D452_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	RuntimeObject* V_1 = NULL;
	int32_t V_2 = 0;
	Exception_t* __last_unhandled_exception = 0;
	il2cpp::utils::ExceptionSupportStack<int32_t, 1> __leave_targets;
	{
		RuntimeObject* L_0 = ___source0;
		if (L_0)
		{
			goto IL_000e;
		}
	}
	{
		Exception_t* L_1;
		L_1 = Error_ArgumentNull_m9157523765DB73FC9F7B984F2F740F2B5EDB7337(((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral66F9618FDA792CAB23AF2D7FFB50AB2D3E393DC5)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Enumerable_Min_m08CFCF12550397A30F094F24E2F68AD21E98EC9F_RuntimeMethod_var)));
	}

IL_000e:
	{
		RuntimeObject* L_2 = ___source0;
		NullCheck(L_2);
		RuntimeObject* L_3;
		L_3 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<T> System.Collections.Generic.IEnumerable`1<System.Int32>::GetEnumerator() */, IEnumerable_1_tCE758D940790D6D0D56B457E522C195F8C413AF2_il2cpp_TypeInfo_var, L_2);
		V_1 = L_3;
	}

IL_0015:
	try
	{// begin try (depth: 1)
		{
			RuntimeObject* L_4 = V_1;
			NullCheck(L_4);
			bool L_5;
			L_5 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_4);
			if (L_5)
			{
				goto IL_0023;
			}
		}

IL_001d:
		{
			Exception_t* L_6;
			L_6 = Error_NoElements_m0E3D3D6CC1E9C9ED28CD989485766C91DC71042F(NULL);
			IL2CPP_RAISE_MANAGED_EXCEPTION(L_6, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Enumerable_Min_m08CFCF12550397A30F094F24E2F68AD21E98EC9F_RuntimeMethod_var)));
		}

IL_0023:
		{
			RuntimeObject* L_7 = V_1;
			NullCheck(L_7);
			int32_t L_8;
			L_8 = InterfaceFuncInvoker0< int32_t >::Invoke(0 /* T System.Collections.Generic.IEnumerator`1<System.Int32>::get_Current() */, IEnumerator_1_tD6A90A7446DA8E6CF865EDFBBF18C1200BB6D452_il2cpp_TypeInfo_var, L_7);
			V_0 = L_8;
			goto IL_0039;
		}

IL_002c:
		{
			RuntimeObject* L_9 = V_1;
			NullCheck(L_9);
			int32_t L_10;
			L_10 = InterfaceFuncInvoker0< int32_t >::Invoke(0 /* T System.Collections.Generic.IEnumerator`1<System.Int32>::get_Current() */, IEnumerator_1_tD6A90A7446DA8E6CF865EDFBBF18C1200BB6D452_il2cpp_TypeInfo_var, L_9);
			V_2 = L_10;
			int32_t L_11 = V_2;
			int32_t L_12 = V_0;
			if ((((int32_t)L_11) >= ((int32_t)L_12)))
			{
				goto IL_0039;
			}
		}

IL_0037:
		{
			int32_t L_13 = V_2;
			V_0 = L_13;
		}

IL_0039:
		{
			RuntimeObject* L_14 = V_1;
			NullCheck(L_14);
			bool L_15;
			L_15 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_14);
			if (L_15)
			{
				goto IL_002c;
			}
		}

IL_0041:
		{
			IL2CPP_LEAVE(0x77, FINALLY_0043);
		}
	}// end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t*)e.ex;
		goto FINALLY_0043;
	}

FINALLY_0043:
	{// begin finally (depth: 1)
		{
			RuntimeObject* L_16 = V_1;
			if (!L_16)
			{
				goto IL_004c;
			}
		}

IL_0046:
		{
			RuntimeObject* L_17 = V_1;
			NullCheck(L_17);
			InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_17);
		}

IL_004c:
		{
			IL2CPP_END_FINALLY(67)
		}
	}// end finally (depth: 1)
	IL2CPP_CLEANUP(67)
	{
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t*)
		IL2CPP_JUMP_TBL(0x77, IL_004d)
	}

IL_004d:
	{
		int32_t L_18 = V_0;
		return L_18;
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
// System.Void System.Collections.Generic.CopyPosition::.ctor(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CopyPosition__ctor_mFDF577B19459EB1D3EB4F7AA29125C2EB8E3604D (CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* __this, int32_t ___row0, int32_t ___column1, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___row0;
		__this->___U3CRowU3Ek__BackingField_0 = L_0;
		int32_t L_1 = ___column1;
		__this->___U3CColumnU3Ek__BackingField_1 = L_1;
		return;
	}
}
IL2CPP_EXTERN_C  void CopyPosition__ctor_mFDF577B19459EB1D3EB4F7AA29125C2EB8E3604D_AdjustorThunk (RuntimeObject* __this, int32_t ___row0, int32_t ___column1, const RuntimeMethod* method)
{
	CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C*>(__this + _offset);
	CopyPosition__ctor_mFDF577B19459EB1D3EB4F7AA29125C2EB8E3604D(_thisAdjusted, ___row0, ___column1, method);
}
// System.Collections.Generic.CopyPosition System.Collections.Generic.CopyPosition::get_Start()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C CopyPosition_get_Start_mC89BBEBF99313B043614B9EFCBB57FBAD3930593 (const RuntimeMethod* method)
{
	CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_initobj((&V_0), sizeof(CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C));
		CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C L_0 = V_0;
		return L_0;
	}
}
// System.Int32 System.Collections.Generic.CopyPosition::get_Row()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t CopyPosition_get_Row_m9D2FDA30FD2F0235CCF3F82EC147B64B8BF44D18 (CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->___U3CRowU3Ek__BackingField_0;
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t CopyPosition_get_Row_m9D2FDA30FD2F0235CCF3F82EC147B64B8BF44D18_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C*>(__this + _offset);
	int32_t _returnValue;
	_returnValue = CopyPosition_get_Row_m9D2FDA30FD2F0235CCF3F82EC147B64B8BF44D18_inline(_thisAdjusted, method);
	return _returnValue;
}
// System.Int32 System.Collections.Generic.CopyPosition::get_Column()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t CopyPosition_get_Column_m217F5811015F2AF2D1381AE8EF5127A2245B6C1E (CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->___U3CColumnU3Ek__BackingField_1;
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t CopyPosition_get_Column_m217F5811015F2AF2D1381AE8EF5127A2245B6C1E_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C*>(__this + _offset);
	int32_t _returnValue;
	_returnValue = CopyPosition_get_Column_m217F5811015F2AF2D1381AE8EF5127A2245B6C1E_inline(_thisAdjusted, method);
	return _returnValue;
}
// System.Collections.Generic.CopyPosition System.Collections.Generic.CopyPosition::Normalize(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C CopyPosition_Normalize_mAB50E4279BC0C6EC003BE90A65EB38521D0CC164 (CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* __this, int32_t ___endColumn0, const RuntimeMethod* method)
{
	{
		int32_t L_0;
		L_0 = CopyPosition_get_Column_m217F5811015F2AF2D1381AE8EF5127A2245B6C1E_inline(__this, NULL);
		int32_t L_1 = ___endColumn0;
		if ((((int32_t)L_0) == ((int32_t)L_1)))
		{
			goto IL_0010;
		}
	}
	{
		CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C L_2 = (*(CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C*)__this);
		return L_2;
	}

IL_0010:
	{
		int32_t L_3;
		L_3 = CopyPosition_get_Row_m9D2FDA30FD2F0235CCF3F82EC147B64B8BF44D18_inline(__this, NULL);
		CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C L_4;
		memset((&L_4), 0, sizeof(L_4));
		CopyPosition__ctor_mFDF577B19459EB1D3EB4F7AA29125C2EB8E3604D((&L_4), ((int32_t)il2cpp_codegen_add((int32_t)L_3, (int32_t)1)), 0, /*hidden argument*/NULL);
		return L_4;
	}
}
IL2CPP_EXTERN_C  CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C CopyPosition_Normalize_mAB50E4279BC0C6EC003BE90A65EB38521D0CC164_AdjustorThunk (RuntimeObject* __this, int32_t ___endColumn0, const RuntimeMethod* method)
{
	CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C*>(__this + _offset);
	CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C _returnValue;
	_returnValue = CopyPosition_Normalize_mAB50E4279BC0C6EC003BE90A65EB38521D0CC164(_thisAdjusted, ___endColumn0, method);
	return _returnValue;
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void System.Collections.Generic.Marker::.ctor(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Marker__ctor_m5A9F92F3092A06075D55D96F770EFAC2EFC2450A (Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5* __this, int32_t ___count0, int32_t ___index1, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___count0;
		__this->___U3CCountU3Ek__BackingField_0 = L_0;
		int32_t L_1 = ___index1;
		__this->___U3CIndexU3Ek__BackingField_1 = L_1;
		return;
	}
}
IL2CPP_EXTERN_C  void Marker__ctor_m5A9F92F3092A06075D55D96F770EFAC2EFC2450A_AdjustorThunk (RuntimeObject* __this, int32_t ___count0, int32_t ___index1, const RuntimeMethod* method)
{
	Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5*>(__this + _offset);
	Marker__ctor_m5A9F92F3092A06075D55D96F770EFAC2EFC2450A(_thisAdjusted, ___count0, ___index1, method);
}
// System.Int32 System.Collections.Generic.Marker::get_Count()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Marker_get_Count_m7EA2D8F123074BC865257FB803537912244BE94E (Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->___U3CCountU3Ek__BackingField_0;
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t Marker_get_Count_m7EA2D8F123074BC865257FB803537912244BE94E_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5*>(__this + _offset);
	int32_t _returnValue;
	_returnValue = Marker_get_Count_m7EA2D8F123074BC865257FB803537912244BE94E_inline(_thisAdjusted, method);
	return _returnValue;
}
// System.Int32 System.Collections.Generic.Marker::get_Index()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Marker_get_Index_mB6E75E1F49ADC7E07FDF4B36029802024A549BD0 (Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->___U3CIndexU3Ek__BackingField_1;
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t Marker_get_Index_mB6E75E1F49ADC7E07FDF4B36029802024A549BD0_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5*>(__this + _offset);
	int32_t _returnValue;
	_returnValue = Marker_get_Index_mB6E75E1F49ADC7E07FDF4B36029802024A549BD0_inline(_thisAdjusted, method);
	return _returnValue;
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool Single_IsNaN_m684B090AA2F895FD91821CA8684CBC11D784E4DD_inline (float ___f0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&BitConverter_t6E99605185963BC12B3D369E13F2B88997E64A27_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		float L_0 = ___f0;
		il2cpp_codegen_runtime_class_init_inline(BitConverter_t6E99605185963BC12B3D369E13F2B88997E64A27_il2cpp_TypeInfo_var);
		int32_t L_1;
		L_1 = BitConverter_SingleToInt32Bits_mA1902D40966CA4C89A8974B10E5680A06E88566B_inline(L_0, NULL);
		return (bool)((((int32_t)((int32_t)((int32_t)L_1&(int32_t)((int32_t)2147483647LL)))) > ((int32_t)((int32_t)2139095040)))? 1 : 0);
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t CopyPosition_get_Row_m9D2FDA30FD2F0235CCF3F82EC147B64B8BF44D18_inline (CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->___U3CRowU3Ek__BackingField_0;
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t CopyPosition_get_Column_m217F5811015F2AF2D1381AE8EF5127A2245B6C1E_inline (CopyPosition_t0AD2CC2387ED9414ED8DDD14049D8C0BF90F965C* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->___U3CColumnU3Ek__BackingField_1;
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t Marker_get_Count_m7EA2D8F123074BC865257FB803537912244BE94E_inline (Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->___U3CCountU3Ek__BackingField_0;
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t Marker_get_Index_mB6E75E1F49ADC7E07FDF4B36029802024A549BD0_inline (Marker_t45B3B44AEE551AD976C87418A0A804D9EE4CDFB5* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->___U3CIndexU3Ek__BackingField_1;
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t BitConverter_SingleToInt32Bits_mA1902D40966CA4C89A8974B10E5680A06E88566B_inline (float ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = *((int32_t*)((uintptr_t)(&___value0)));
		return L_0;
	}
}
