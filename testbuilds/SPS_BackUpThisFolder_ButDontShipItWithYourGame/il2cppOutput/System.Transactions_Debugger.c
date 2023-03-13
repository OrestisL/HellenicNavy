#include "pch-c.h"
#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include "codegen/il2cpp-codegen-metadata.h"





#if IL2CPP_MONO_DEBUGGER
static const Il2CppMethodExecutionContextInfo g_methodExecutionContextInfos[1] = { { 0, 0, 0 } };
#else
static const Il2CppMethodExecutionContextInfo g_methodExecutionContextInfos[1] = { { 0, 0, 0 } };
#endif
#if IL2CPP_MONO_DEBUGGER
static const char* g_methodExecutionContextInfoStrings[1] = { NULL };
#else
static const char* g_methodExecutionContextInfoStrings[1] = { NULL };
#endif
#if IL2CPP_MONO_DEBUGGER
static const Il2CppMethodExecutionContextInfoIndex g_methodExecutionContextInfoIndexes[50] = 
{
	{ 0, 0 } /* 0x06000001 System.Void System.MonoTODOAttribute::.ctor() */,
	{ 0, 0 } /* 0x06000002 System.Void System.MonoTODOAttribute::.ctor(System.String) */,
	{ 0, 0 } /* 0x06000003 System.Void System.Transactions.TransactionCompletedEventHandler::.ctor(System.Object,System.IntPtr) */,
	{ 0, 0 } /* 0x06000004 System.Void System.Transactions.TransactionCompletedEventHandler::Invoke(System.Object,System.Transactions.TransactionEventArgs) */,
	{ 0, 0 } /* 0x06000005 System.Void System.Transactions.Enlistment::.ctor() */,
	{ 0, 0 } /* 0x06000006 System.Void System.Transactions.Enlistment::Done() */,
	{ 0, 0 } /* 0x06000007 System.Void System.Transactions.Enlistment::InternalOnDone() */,
	{ 0, 0 } /* 0x06000008 System.Void System.Transactions.IEnlistmentNotification::Rollback(System.Transactions.Enlistment) */,
	{ 0, 0 } /* 0x06000009 System.Void System.Transactions.IPromotableSinglePhaseNotification::Rollback(System.Transactions.SinglePhaseEnlistment) */,
	{ 0, 0 } /* 0x0600000A System.Void System.Transactions.SinglePhaseEnlistment::.ctor() */,
	{ 0, 0 } /* 0x0600000B System.Collections.Generic.List`1<System.Transactions.IEnlistmentNotification> System.Transactions.Transaction::get_Volatiles() */,
	{ 0, 0 } /* 0x0600000C System.Collections.Generic.List`1<System.Transactions.ISinglePhaseNotification> System.Transactions.Transaction::get_Durables() */,
	{ 0, 0 } /* 0x0600000D System.Void System.Transactions.Transaction::System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) */,
	{ 0, 0 } /* 0x0600000E System.Transactions.Transaction System.Transactions.Transaction::get_Current() */,
	{ 0, 0 } /* 0x0600000F System.Transactions.Transaction System.Transactions.Transaction::get_CurrentInternal() */,
	{ 0, 0 } /* 0x06000010 System.Transactions.TransactionInformation System.Transactions.Transaction::get_TransactionInformation() */,
	{ 0, 0 } /* 0x06000011 System.Void System.Transactions.Transaction::Dispose() */,
	{ 0, 0 } /* 0x06000012 System.Transactions.Enlistment System.Transactions.Transaction::EnlistVolatile(System.Transactions.IEnlistmentNotification,System.Transactions.EnlistmentOptions) */,
	{ 0, 0 } /* 0x06000013 System.Transactions.Enlistment System.Transactions.Transaction::EnlistVolatileInternal(System.Transactions.IEnlistmentNotification,System.Transactions.EnlistmentOptions) */,
	{ 0, 0 } /* 0x06000014 System.Boolean System.Transactions.Transaction::Equals(System.Object) */,
	{ 0, 0 } /* 0x06000015 System.Boolean System.Transactions.Transaction::Equals(System.Transactions.Transaction) */,
	{ 0, 0 } /* 0x06000016 System.Boolean System.Transactions.Transaction::op_Equality(System.Transactions.Transaction,System.Transactions.Transaction) */,
	{ 0, 0 } /* 0x06000017 System.Boolean System.Transactions.Transaction::op_Inequality(System.Transactions.Transaction,System.Transactions.Transaction) */,
	{ 0, 0 } /* 0x06000018 System.Int32 System.Transactions.Transaction::GetHashCode() */,
	{ 0, 0 } /* 0x06000019 System.Void System.Transactions.Transaction::Rollback() */,
	{ 0, 0 } /* 0x0600001A System.Void System.Transactions.Transaction::Rollback(System.Exception) */,
	{ 0, 0 } /* 0x0600001B System.Void System.Transactions.Transaction::Rollback(System.Exception,System.Object) */,
	{ 0, 0 } /* 0x0600001C System.Void System.Transactions.Transaction::set_Aborted(System.Boolean) */,
	{ 0, 0 } /* 0x0600001D System.Transactions.TransactionScope System.Transactions.Transaction::get_Scope() */,
	{ 0, 0 } /* 0x0600001E System.Void System.Transactions.Transaction::FireCompleted() */,
	{ 0, 0 } /* 0x0600001F System.Void System.Transactions.Transaction::EnsureIncompleteCurrentScope() */,
	{ 0, 0 } /* 0x06000020 System.Void System.Transactions.Transaction::.ctor() */,
	{ 0, 0 } /* 0x06000021 System.Void System.Transactions.Transaction/AsyncCommit::.ctor(System.Object,System.IntPtr) */,
	{ 0, 0 } /* 0x06000022 System.Void System.Transactions.Transaction/AsyncCommit::Invoke() */,
	{ 0, 0 } /* 0x06000023 System.Void System.Transactions.TransactionEventArgs::.ctor() */,
	{ 0, 0 } /* 0x06000024 System.Void System.Transactions.TransactionEventArgs::.ctor(System.Transactions.Transaction) */,
	{ 0, 0 } /* 0x06000025 System.Void System.Transactions.TransactionException::.ctor() */,
	{ 0, 0 } /* 0x06000026 System.Void System.Transactions.TransactionException::.ctor(System.String) */,
	{ 0, 0 } /* 0x06000027 System.Void System.Transactions.TransactionException::.ctor(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) */,
	{ 0, 0 } /* 0x06000028 System.Transactions.TransactionStatus System.Transactions.TransactionInformation::get_Status() */,
	{ 0, 0 } /* 0x06000029 System.Void System.Transactions.TransactionInformation::set_Status(System.Transactions.TransactionStatus) */,
	{ 0, 0 } /* 0x0600002A System.Void System.Transactions.TransactionManager::.cctor() */,
	{ 0, 0 } /* 0x0600002B System.TimeSpan System.Transactions.TransactionManager::get_DefaultTimeout() */,
	{ 0, 0 } /* 0x0600002C System.Void System.Transactions.TransactionOptions::.ctor(System.Transactions.IsolationLevel,System.TimeSpan) */,
	{ 0, 0 } /* 0x0600002D System.Boolean System.Transactions.TransactionOptions::op_Equality(System.Transactions.TransactionOptions,System.Transactions.TransactionOptions) */,
	{ 0, 0 } /* 0x0600002E System.Boolean System.Transactions.TransactionOptions::Equals(System.Object) */,
	{ 0, 0 } /* 0x0600002F System.Int32 System.Transactions.TransactionOptions::GetHashCode() */,
	{ 0, 0 } /* 0x06000030 System.Boolean System.Transactions.TransactionScope::get_IsComplete() */,
	{ 0, 0 } /* 0x06000031 System.Void System.Transactions.TransactionScope::.cctor() */,
	{ 0, 0 } /* 0x06000032 System.Void Unity.ThrowStub::ThrowNotSupportedException() */,
};
#else
static const Il2CppMethodExecutionContextInfoIndex g_methodExecutionContextInfoIndexes[1] = { { 0, 0} };
#endif
#if IL2CPP_MONO_DEBUGGER
extern Il2CppSequencePoint g_sequencePointsSystem_Transactions[];
Il2CppSequencePoint g_sequencePointsSystem_Transactions[1] = { { 0, 0, 0, 0, 0, 0, 0, kSequencePointKind_Normal, 0, 0, } };
#else
extern Il2CppSequencePoint g_sequencePointsSystem_Transactions[];
Il2CppSequencePoint g_sequencePointsSystem_Transactions[1] = { { 0, 0, 0, 0, 0, 0, 0, kSequencePointKind_Normal, 0, 0, } };
#endif
#if IL2CPP_MONO_DEBUGGER
static const Il2CppCatchPoint g_catchPoints[1] = { { 0, 0, 0, 0, } };
#else
static const Il2CppCatchPoint g_catchPoints[1] = { { 0, 0, 0, 0, } };
#endif
#if IL2CPP_MONO_DEBUGGER
static const Il2CppSequencePointSourceFile g_sequencePointSourceFiles[1] = { NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
#else
static const Il2CppSequencePointSourceFile g_sequencePointSourceFiles[1] = { NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
#endif
#if IL2CPP_MONO_DEBUGGER
static const Il2CppTypeSourceFilePair g_typeSourceFiles[1] = { { 0, 0 } };
#else
static const Il2CppTypeSourceFilePair g_typeSourceFiles[1] = { { 0, 0 } };
#endif
#if IL2CPP_MONO_DEBUGGER
static const Il2CppMethodScope g_methodScopes[1] = { { 0, 0 } };
#else
static const Il2CppMethodScope g_methodScopes[1] = { { 0, 0 } };
#endif
#if IL2CPP_MONO_DEBUGGER
static const Il2CppMethodHeaderInfo g_methodHeaderInfos[50] = 
{
	{ 0, 0, 0 } /* System.Void System.MonoTODOAttribute::.ctor() */,
	{ 0, 0, 0 } /* System.Void System.MonoTODOAttribute::.ctor(System.String) */,
	{ 0, 0, 0 } /* System.Void System.Transactions.TransactionCompletedEventHandler::.ctor(System.Object,System.IntPtr) */,
	{ 0, 0, 0 } /* System.Void System.Transactions.TransactionCompletedEventHandler::Invoke(System.Object,System.Transactions.TransactionEventArgs) */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Enlistment::.ctor() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Enlistment::Done() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Enlistment::InternalOnDone() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.IEnlistmentNotification::Rollback(System.Transactions.Enlistment) */,
	{ 0, 0, 0 } /* System.Void System.Transactions.IPromotableSinglePhaseNotification::Rollback(System.Transactions.SinglePhaseEnlistment) */,
	{ 0, 0, 0 } /* System.Void System.Transactions.SinglePhaseEnlistment::.ctor() */,
	{ 0, 0, 0 } /* System.Collections.Generic.List`1<System.Transactions.IEnlistmentNotification> System.Transactions.Transaction::get_Volatiles() */,
	{ 0, 0, 0 } /* System.Collections.Generic.List`1<System.Transactions.ISinglePhaseNotification> System.Transactions.Transaction::get_Durables() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Transaction::System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) */,
	{ 0, 0, 0 } /* System.Transactions.Transaction System.Transactions.Transaction::get_Current() */,
	{ 0, 0, 0 } /* System.Transactions.Transaction System.Transactions.Transaction::get_CurrentInternal() */,
	{ 0, 0, 0 } /* System.Transactions.TransactionInformation System.Transactions.Transaction::get_TransactionInformation() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Transaction::Dispose() */,
	{ 0, 0, 0 } /* System.Transactions.Enlistment System.Transactions.Transaction::EnlistVolatile(System.Transactions.IEnlistmentNotification,System.Transactions.EnlistmentOptions) */,
	{ 0, 0, 0 } /* System.Transactions.Enlistment System.Transactions.Transaction::EnlistVolatileInternal(System.Transactions.IEnlistmentNotification,System.Transactions.EnlistmentOptions) */,
	{ 0, 0, 0 } /* System.Boolean System.Transactions.Transaction::Equals(System.Object) */,
	{ 0, 0, 0 } /* System.Boolean System.Transactions.Transaction::Equals(System.Transactions.Transaction) */,
	{ 0, 0, 0 } /* System.Boolean System.Transactions.Transaction::op_Equality(System.Transactions.Transaction,System.Transactions.Transaction) */,
	{ 0, 0, 0 } /* System.Boolean System.Transactions.Transaction::op_Inequality(System.Transactions.Transaction,System.Transactions.Transaction) */,
	{ 0, 0, 0 } /* System.Int32 System.Transactions.Transaction::GetHashCode() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Transaction::Rollback() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Transaction::Rollback(System.Exception) */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Transaction::Rollback(System.Exception,System.Object) */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Transaction::set_Aborted(System.Boolean) */,
	{ 0, 0, 0 } /* System.Transactions.TransactionScope System.Transactions.Transaction::get_Scope() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Transaction::FireCompleted() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Transaction::EnsureIncompleteCurrentScope() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Transaction::.ctor() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Transaction/AsyncCommit::.ctor(System.Object,System.IntPtr) */,
	{ 0, 0, 0 } /* System.Void System.Transactions.Transaction/AsyncCommit::Invoke() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.TransactionEventArgs::.ctor() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.TransactionEventArgs::.ctor(System.Transactions.Transaction) */,
	{ 0, 0, 0 } /* System.Void System.Transactions.TransactionException::.ctor() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.TransactionException::.ctor(System.String) */,
	{ 0, 0, 0 } /* System.Void System.Transactions.TransactionException::.ctor(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) */,
	{ 0, 0, 0 } /* System.Transactions.TransactionStatus System.Transactions.TransactionInformation::get_Status() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.TransactionInformation::set_Status(System.Transactions.TransactionStatus) */,
	{ 0, 0, 0 } /* System.Void System.Transactions.TransactionManager::.cctor() */,
	{ 0, 0, 0 } /* System.TimeSpan System.Transactions.TransactionManager::get_DefaultTimeout() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.TransactionOptions::.ctor(System.Transactions.IsolationLevel,System.TimeSpan) */,
	{ 0, 0, 0 } /* System.Boolean System.Transactions.TransactionOptions::op_Equality(System.Transactions.TransactionOptions,System.Transactions.TransactionOptions) */,
	{ 0, 0, 0 } /* System.Boolean System.Transactions.TransactionOptions::Equals(System.Object) */,
	{ 0, 0, 0 } /* System.Int32 System.Transactions.TransactionOptions::GetHashCode() */,
	{ 0, 0, 0 } /* System.Boolean System.Transactions.TransactionScope::get_IsComplete() */,
	{ 0, 0, 0 } /* System.Void System.Transactions.TransactionScope::.cctor() */,
	{ 0, 0, 0 } /* System.Void Unity.ThrowStub::ThrowNotSupportedException() */,
};
#else
static const Il2CppMethodHeaderInfo g_methodHeaderInfos[1] = { { 0, 0, 0 } };
#endif
IL2CPP_EXTERN_C const Il2CppDebuggerMetadataRegistration g_DebuggerMetadataRegistrationSystem_Transactions;
const Il2CppDebuggerMetadataRegistration g_DebuggerMetadataRegistrationSystem_Transactions = 
{
	(Il2CppMethodExecutionContextInfo*)g_methodExecutionContextInfos,
	(Il2CppMethodExecutionContextInfoIndex*)g_methodExecutionContextInfoIndexes,
	(Il2CppMethodScope*)g_methodScopes,
	(Il2CppMethodHeaderInfo*)g_methodHeaderInfos,
	(Il2CppSequencePointSourceFile*)g_sequencePointSourceFiles,
	0,
	(Il2CppSequencePoint*)g_sequencePointsSystem_Transactions,
	0,
	(Il2CppCatchPoint*)g_catchPoints,
	0,
	(Il2CppTypeSourceFilePair*)g_typeSourceFiles,
	(const char**)g_methodExecutionContextInfoStrings,
};
