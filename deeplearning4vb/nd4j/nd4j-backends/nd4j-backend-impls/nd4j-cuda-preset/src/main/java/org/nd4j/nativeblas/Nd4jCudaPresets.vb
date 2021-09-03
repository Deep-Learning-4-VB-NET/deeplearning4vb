Imports System
Imports System.Collections.Generic
Imports ClassProperties = org.bytedeco.javacpp.ClassProperties
Imports LoadEnabled = org.bytedeco.javacpp.LoadEnabled
Imports Loader = org.bytedeco.javacpp.Loader
Imports Platform = org.bytedeco.javacpp.annotation.Platform
Imports Properties = org.bytedeco.javacpp.annotation.Properties
Imports Info = org.bytedeco.javacpp.tools.Info
Imports InfoMap = org.bytedeco.javacpp.tools.InfoMap
Imports InfoMapper = org.bytedeco.javacpp.tools.InfoMapper

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.nativeblas

	''' 
	''' <summary>
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Properties(target = "org.nd4j.nativeblas.Nd4jCuda", helper = "org.nd4j.nativeblas.Nd4jCudaHelper", value = {@Platform(define = "LIBND4J_ALL_OPS", include = { "array/DataType.h", "array/DataBuffer.h", "array/PointerDeallocator.h", "array/PointerWrapper.h", "array/ConstantDescriptor.h", "array/ConstantDataBuffer.h", "array/ConstantShapeBuffer.h", "array/ConstantOffsetsBuffer.h", "array/TadPack.h", "execution/ErrorReference.h", "execution/Engine.h", "execution/ExecutionMode.h", "memory/MemoryType.h", "system/Environment.h", "types/utf8string.h", "legacy/NativeOps.h", "memory/ExternalWorkspace.h", "memory/Workspace.h", "indexing/NDIndex.h", "indexing/IndicesList.h", "array/DataType.h", "graph/VariableType.h", "graph/ArgumentsList.h", "types/pair.h", "types/pair.h", "array/NDArray.h", "array/NDArrayList.h", "array/ResultSet.h", "graph/RandomGenerator.h", "graph/Variable.h", "graph/VariablesSet.h", "graph/FlowPath.h", "graph/Intervals.h", "graph/Stash.h", "graph/GraphState.h", "graph/VariableSpace.h", "helpers/helper_generator.h", "graph/profiling/GraphProfile.h", "graph/profiling/NodeProfile.h", "graph/Context.h", "graph/ContextPrototype.h", "graph/ResultWrapper.h", "helpers/shape.h", "array/ShapeList.h", "ops/InputType.h", "ops/declarable/OpDescriptor.h", "ops/declarable/PlatformHelper.h", "ops/declarable/BroadcastableOp.h", "ops/declarable/BroadcastableBoolOp.h", "helpers/OpArgsHolder.h", "ops/declarable/DeclarableOp.h", "ops/declarable/DeclarableListOp.h", "ops/declarable/DeclarableReductionOp.h", "ops/declarable/DeclarableCustomOp.h", "ops/declarable/BooleanOp.h", "ops/declarable/LogicOp.h", "ops/declarable/OpRegistrator.h", "execution/ContextBuffers.h", "execution/LaunchContext.h", "array/ShapeDescriptor.h", "array/TadDescriptor.h", "array/TadPack.h", "helpers/DebugInfo.h", "ops/declarable/CustomOperations.h", "build_info.h"}, exclude = {"ops/declarable/headers/activations.h", "ops/declarable/headers/boolean.h", "ops/declarable/headers/broadcastable.h", "ops/declarable/headers/convo.h", "ops/declarable/headers/list.h", "ops/declarable/headers/recurrent.h", "ops/declarable/headers/transforms.h", "ops/declarable/headers/parity_ops.h", "ops/declarable/headers/shape.h", "ops/declarable/headers/random.h", "ops/declarable/headers/nn.h", "ops/declarable/headers/blas.h", "ops/declarable/headers/bitwise.h", "ops/declarable/headers/tests.h", "ops/declarable/headers/loss.h", "ops/declarable/headers/datatypes.h", "ops/declarable/headers/third_party.h", "cnpy/cnpy.h" }, compiler = {"cpp11", "nowarnings"}, library = "jnind4jcuda", link = "nd4jcuda", preload = "libnd4jcuda"), @Platform(value = "linux", preload = "gomp@.1", preloadpath = {"/lib64/", "/lib/", "/usr/lib64/", "/usr/lib/"}), @Platform(value = "linux-armhf", preloadpath = {"/usr/arm-linux-gnueabihf/lib/", "/usr/lib/arm-linux-gnueabihf/"}), @Platform(value = "linux-arm64", preloadpath = {"/usr/aarch64-linux-gnu/lib/", "/usr/lib/aarch64-linux-gnu/"}), @Platform(value = "linux-ppc64", preloadpath = {"/usr/powerpc64-linux-gnu/lib/", "/usr/powerpc64le-linux-gnu/lib/", "/usr/lib/powerpc64-linux-gnu/", "/usr/lib/powerpc64le-linux-gnu/"}), @Platform(value = "windows", preload = {"libwinpthread-1", "libgcc_s_seh-1", "libgomp-1", "libstdc++-6", "libnd4jcpu"}), @Platform(extension = {"-cudnn","-"})}) public class Nd4jCudaPresets implements org.bytedeco.javacpp.LoadEnabled, org.bytedeco.javacpp.tools.InfoMapper
	Public Class Nd4jCudaPresets
		Implements LoadEnabled, InfoMapper

		Public Overrides Sub init(ByVal properties As ClassProperties)
			Dim platform As String = properties.getProperty("platform")
			Dim preloads As IList(Of String) = properties.get("platform.preload")
			Dim resources As IList(Of String) = properties.get("platform.preloadresource")

			' Only apply this at load time since we don't want to copy the CUDA libraries here
			If Not Loader.isLoadLibraries() Then
				Return
			End If
			Dim i As Integer = 0
			Dim libs() As String = {"cudart", "cublasLt", "cublas", "curand", "cusolver", "cusparse", "cudnn", "cudnn_ops_infer", "cudnn_ops_train", "cudnn_adv_infer", "cudnn_adv_train", "cudnn_cnn_infer", "cudnn_cnn_train"}
			For Each [lib] As String In libs
				If platform.StartsWith("linux", StringComparison.Ordinal) Then
					[lib] &= If([lib].StartsWith("cudnn", StringComparison.Ordinal), "@.8", If([lib].Equals("curand"), "@.10", If([lib].Equals("cudart"), "@.11.0", "@.11")))
				ElseIf platform.StartsWith("windows", StringComparison.Ordinal) Then
					[lib] &= If([lib].StartsWith("cudnn", StringComparison.Ordinal), "64_8", If([lib].Equals("curand"), "64_10", If([lib].Equals("cudart"), "64_110", "64_11")))
				Else
					Continue For ' no CUDA
				End If
				If Not preloads.Contains([lib]) Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: preloads.add(i++, lib);
					preloads.Insert(i, [lib])
						i += 1
				End If
			Next [lib]
			If i > 0 Then
				resources.Add("/org/bytedeco/cuda/")
			End If
		End Sub

		Public Overrides Sub map(ByVal infoMap As InfoMap)
			infoMap.put((New Info("thread_local", "ND4J_EXPORT", "INLINEDEF", "CUBLASWINAPI", "FORCEINLINE", "_CUDA_H", "_CUDA_D", "_CUDA_G", "_CUDA_HD", "LIBND4J_ALL_OPS", "NOT_EXCLUDED")).cppTypes().annotations()).put((New Info("NativeOps.h", "build_info.h")).objectify()).put((New Info("OpaqueTadPack")).pointerTypes("OpaqueTadPack")).put((New Info("OpaqueResultWrapper")).pointerTypes("OpaqueResultWrapper")).put((New Info("OpaqueShapeList")).pointerTypes("OpaqueShapeList")).put((New Info("OpaqueVariablesSet")).pointerTypes("OpaqueVariablesSet")).put((New Info("OpaqueVariable")).pointerTypes("OpaqueVariable")).put((New Info("OpaqueConstantDataBuffer")).pointerTypes("OpaqueConstantDataBuffer")).put((New Info("OpaqueConstantShapeBuffer")).pointerTypes("OpaqueConstantShapeBuffer")).put((New Info("OpaqueConstantOffsetsBuffer")).pointerTypes("OpaqueConstantOffsetsBuffer")).put((New Info("OpaqueContext")).pointerTypes("OpaqueContext")).put((New Info("OpaqueRandomGenerator")).pointerTypes("OpaqueRandomGenerator")).put((New Info("OpaqueLaunchContext")).pointerTypes("OpaqueLaunchContext")).put((New Info("OpaqueDataBuffer")).pointerTypes("OpaqueDataBuffer")).put((New Info("const char")).valueTypes("byte").pointerTypes("@Cast(""char*"") String", "@Cast(""char*"") BytePointer")).put((New Info("char")).valueTypes("char").pointerTypes("@Cast(""char*"") BytePointer", "@Cast(""char*"") String")).put((New Info("Nd4jPointer")).cast().valueTypes("Pointer").pointerTypes("PointerPointer")).put((New Info("Nd4jLong")).cast().valueTypes("long").pointerTypes("LongPointer", "LongBuffer", "long[]")).put((New Info("Nd4jStatus")).cast().valueTypes("int").pointerTypes("IntPointer", "IntBuffer", "int[]")).put((New Info("float16")).cast().valueTypes("short").pointerTypes("ShortPointer", "ShortBuffer", "short[]")).put((New Info("bfloat16")).cast().valueTypes("short").pointerTypes("ShortPointer", "ShortBuffer", "short[]"))

			infoMap.put((New Info("__CUDACC__", "MAX_UINT", "HAVE_MKLDNN")).define(False)).put((New Info("__JAVACPP_HACK__", "LIBND4J_ALL_OPS","__CUDABLAS__")).define(True)).put((New Info("std::initializer_list", "cnpy::NpyArray", "sd::NDArray::applyLambda", "sd::NDArray::applyPairwiseLambda", "sd::graph::FlatResult", "sd::graph::FlatVariable", "sd::NDArray::subarray", "std::shared_ptr", "sd::PointerWrapper", "sd::PointerDeallocator")).skip()).put((New Info("std::string")).annotations("@StdString").valueTypes("BytePointer", "String").pointerTypes("@Cast({""char*"", ""std::string*""}) BytePointer")).put((New Info("std::pair<int,int>")).pointerTypes("IntIntPair").define()).put((New Info("std::vector<std::vector<int> >")).pointerTypes("IntVectorVector").define()).put((New Info("std::vector<std::vector<Nd4jLong> >")).pointerTypes("LongVectorVector").define()).put((New Info("std::vector<sd::NDArray*>")).pointerTypes("NDArrayVector").define()).put((New Info("std::vector<const sd::NDArray*>")).pointerTypes("ConstNDArrayVector").define()).put((New Info("bool")).cast().valueTypes("boolean").pointerTypes("BooleanPointer", "boolean[]")).put((New Info("sd::graph::ResultWrapper")).base("org.nd4j.nativeblas.ResultWrapperAbstraction").define()).put((New Info("sd::IndicesList")).purify())
	'
	'        String classTemplates[] = {
	'                "sd::NDArray",
	'                "sd::NDArrayList",
	'                "sd::ResultSet",
	'                "sd::OpArgsHolder",
	'                "sd::graph::GraphState",
	'                "sd::graph::Variable",
	'                "sd::graph::VariablesSet",
	'                "sd::graph::Stash",
	'                "sd::graph::VariableSpace",
	'                "sd::graph::Context",
	'                "sd::graph::ContextPrototype",
	'                "sd::ops::DeclarableOp",
	'                "sd::ops::DeclarableListOp",
	'                "sd::ops::DeclarableReductionOp",
	'                "sd::ops::DeclarableCustomOp",
	'                "sd::ops::BooleanOp",
	'                "sd::ops::BroadcastableOp",
	'                "sd::ops::LogicOp"};
	'        for (String t : classTemplates) {
	'            String s = t.substring(t.lastIndexOf(':') + 1);
	'            infoMap.put(new Info(t + "<float>").pointerTypes("Float" + s))
	'                    .put(new Info(t + "<float16>").pointerTypes("Half" + s))
	'                    .put(new Info(t + "<double>").pointerTypes("Double" + s));
	'
	'        }
	'
			infoMap.put((New Info("sd::ops::OpRegistrator::updateMSVC")).skip())
		End Sub
	End Class

End Namespace