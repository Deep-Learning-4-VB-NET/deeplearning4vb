Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports SneakyThrows = lombok.SneakyThrows
Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports OpDeclarationDescriptor = org.nd4j.descriptor.OpDeclarationDescriptor
Imports ArgDescriptorProposal = org.nd4j.descriptor.proposal.ArgDescriptorProposal
Imports ArgDescriptorSource = org.nd4j.descriptor.proposal.ArgDescriptorSource
Imports OpNamespace = org.nd4j.ir.OpNamespace
Imports org.nd4j.descriptor.proposal.impl.ArgDescriptorParserUtils

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.descriptor.proposal.impl



	Public Class Libnd4jArgDescriptorSource
		Implements ArgDescriptorSource

		Private libnd4jPath As String
		Private libnd4jRootDir As File
		Private weight As Double

		Public Const OP_IMPL As String = "OP_IMPL"
		Public Const DIVERGENT_OP_IMPL As String = "DIVERGENT_OP_IMPL"
		Public Const CONFIGURABLE_OP_IMPL As String = "CONFIGURABLE_OP_IMPL"
		Public Const REDUCTION_OP_IMPL As String = "REDUCTION_OP_IMPL"
		Public Const BROADCASTABLE_OP_IMPL As String = "BROADCASTABLE_OP_IMPL"
		Public Const BROADCASTABLE_BOOL_OP_IMPL As String = "BROADCASTABLE_BOOL_OP_IMPL"
		Public Const PLATFORM_IMPL As String = "PLATFORM_IMPL"
		Public Const [RETURN] As String = "return"
		Public Const INT_ARG As String = "INT_ARG"
		Public Const I_ARG As String = "I_ARG"
		Public Const INPUT_VARIABLE As String = "INPUT_VARIABLE"
		Public Const OUTPUT_VARIABLE As String = "OUTPUT_VARIABLE"
		Public Const OUTPUT_NULLIFIED As String = "OUTPUT_NULLIFIED"
		Public Const INPUT_LIST As String = "INPUT_LIST"
		Public Const T_ARG As String = "T_ARG"
		Public Const B_ARG As String = "B_ARG"
		Public Const DECLARE_SYN As String = "DECLARE_SYN"
		Public Const DEFAULT_LIBND4J_DIRECTORY As String = "../../../libnd4j"
		Public Const BROADCASTABLE_OP_IMPL_DEFAULT_NIN As Integer = 2
		Public Const BROADCASTABLE_OP_IMPL_DEFAULT_NOUT As Integer = 1
		Public Const CUSTOM_OP_IMPL As String = "CUSTOM_OP_IMPL"
		Public Const BOOLEAN_OP_IMPL As String = "BOOLEAN_OP_IMPL"
		Public Const LIST_OP_IMPL As String = "LIST_OP_IMPL"
		Public Const LOGIC_OP_IMPL As String = "LOGIC_OP_IMPL"
		'note this allows either a declaration like: auto variableNum = SOME_DECLARATION(0); or auto variableNum = SOME_DECLARATION(0) == 1;
		Public Const ARG_DECLARATION As String = "(\w+\s)+\w+\s*=\s*[A-Z]+_[A-Z]+\(\d+\);"
		Public Const ARG_BOOL_EQUALS_DECLARATION As String = "(\w+\s)+\w+\s*=\s*[A-Z]+_[A-Z]+\(\d+\)\s*==\s*\d+;"
		Public Const ARG_DECLARATION_WITH_VARIABLE As String = "(\w+\s)+\w+\s*=\s*[A-Z]+_[A-Z]+\([\d\w\+-*\/]+);"
		Public Const ARRAY_ASSIGNMENT As String = "\w+\[[\w\d]\]\s*=\s*[A-Z]+_[A-Z]+\s*\([\w\d\+\-\*\/\s]+\);"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default @Getter private Map<String, org.nd4j.ir.OpNamespace.OpDescriptor.OpDeclarationType> opTypes = new HashMap<>();
		Private opTypes As IDictionary(Of String, OpNamespace.OpDescriptor.OpDeclarationType) = New Dictionary(Of String, OpNamespace.OpDescriptor.OpDeclarationType)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public Libnd4jArgDescriptorSource(String libnd4jPath,double weight)
		Public Sub New(ByVal libnd4jPath As String, ByVal weight As Double)
			If libnd4jPath Is Nothing Then
				libnd4jPath = "../libnd4j"
			End If
			If weight = 0 Then
				weight = 999
			End If
			Me.weight = weight
			libnd4jRootDir = New File(libnd4jPath)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SneakyThrows public Map<String, List<org.nd4j.descriptor.proposal.ArgDescriptorProposal>> doExtractArgDescriptors()
		Public Overridable Function doExtractArgDescriptors() As IDictionary(Of String, IList(Of ArgDescriptorProposal))
			Dim ret As IDictionary(Of String, IList(Of ArgDescriptorProposal)) = New Dictionary(Of String, IList(Of ArgDescriptorProposal))()
			Dim opDeclarationDescriptors As IList(Of OpDeclarationDescriptor) = New List(Of OpDeclarationDescriptor)()
			Dim descriptorMap As IDictionary(Of String, OpDeclarationDescriptor) = New Dictionary(Of String, OpDeclarationDescriptor)()
			'only include/ops the include directory, otherwise other misc folders get scanned
			Files.walk((New File(libnd4jRootDir,"include/ops")).toPath(), New FileVisitOption(){ FileVisitOption.FOLLOW_LINKS }).filter(Function(path) path.toFile().getAbsolutePath().EndsWith(".cpp")).forEach(Sub(path)
			Try
				Dim lines As IList(Of String) = Files.readAllLines(path)
				Dim inOpBlock As Boolean = False
				Dim foundOp As Boolean = False
				Dim oneLineOp As Boolean = False
				Dim inArgNames As IList(Of String) = New List(Of String)()
				Dim outArgNames As IList(Of String) = New List(Of String)()
				Dim tArgNames As IList(Of String) = New List(Of String)()
				Dim iArgNames As IList(Of String) = New List(Of String)()
				Dim bArgNames As IList(Of String) = New List(Of String)()
				Dim inArgIndices As IList(Of Integer) = New List(Of Integer)()
				Dim outArgIndices As IList(Of Integer) = New List(Of Integer)()
				Dim tArgIndices As IList(Of Integer) = New List(Of Integer)()
				Dim iArgIndices As IList(Of Integer) = New List(Of Integer)()
				Dim bArgIndices As IList(Of Integer) = New List(Of Integer)()
				Dim opDeclarationDescriptor As OpDeclarationDescriptor = Nothing
				Dim builder As OpDeclarationDescriptor.OpDeclarationDescriptorBuilder = OpDeclarationDescriptor.builder()
				Dim currentOpNin As Integer = -1, currentOpNout As Integer = -1, currentOpIntArgs As Integer = -1, currentOutTArgs As Integer = -1, currentOpBooleanArgs As Integer = -1
				Dim hasNin As Boolean = False, hasNout As Boolean = False, hasIntArgs As Boolean = False, hasTArgs As Boolean = False, platformImpl As Boolean = False
				Dim argDescriptorProposals As IList(Of ArgDescriptorProposal) = Nothing
				Dim currLineIdx As Integer = 0
				Dim name As String = Nothing
				For Each line As String In lines
					If line.Trim().Length = 0 OrElse line.Trim().StartsWith("//", StringComparison.Ordinal) OrElse line.Trim().Length = 1 OrElse line.Trim().Length = 0 Then
						currLineIdx += 1
						Continue For
					End If
					If Not inOpBlock Then
						If line.Contains(CUSTOM_OP_IMPL) Then
							foundOp = True
							line = removeBracesFromDeclarationMacro(line, CUSTOM_OP_IMPL)
							Dim split() As String = line.Trim().Split(",", True)
							name = split(0)
							opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.CUSTOM_OP_IMPL
							argDescriptorProposals = New List(Of ArgDescriptorProposal)()
							If Not name.Equals("randomuniform") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(9999999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DATA_TYPE).setName("dtype").setIsArray(False).setArgIndex(0).build()).build())
							Else
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(9999999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DATA_TYPE).setName("dataType").setIsArray(False).setArgIndex(0).build()).build())
							End If
							If name.Equals("split") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("numSplit").setIsArray(False).setArgIndex(0).build()).build())
							End If
							If name.Equals("resize_bilinear") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(99999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setArgType(OpNamespace.ArgDescriptor.ArgType.BOOL).setName("alignCorners").build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(99999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(1).setArgType(OpNamespace.ArgDescriptor.ArgType.BOOL).setName("halfPixelCenters").build()).build())
							End If
							If name.Equals("split_v") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("numSplit").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("numSplit").setIsArray(False).setArgIndex(1).build()).build())
							End If
							If name.Equals("concat") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("isAxisInLastArr").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.BOOL).setName("isDynamicAxis").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("concatDimension").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("concatDimension").setIsArray(False).setArgIndex(1).build()).build())
							End If
							If name.Equals("dynamic_partition") OrElse name.Equals("dynamic_stitch") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("numPartitions").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("numPartitions").setIsArray(False).setArgIndex(0).build()).build())
							End If
							If name.Equals("dilation2d") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("isSameMode").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("isSameMode").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("rates").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("rates").setIsArray(True).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("strides").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("strides").setIsArray(True).setArgIndex(2).build()).build())
							End If
							If name.Equals("extract_image_patches") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("isSameMode").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.BOOL).setName("isSameMode").setIsArray(False).setArgIndex(0).build()).build())
							End If
							If name.Equals("bincount") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DATA_TYPE).setName("outputType").setIsArray(True).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("values").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("weights").setIsArray(False).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("min").setIsArray(False).setArgIndex(2).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("max").setIsArray(False).setArgIndex(3).build()).build())
							End If
							If name.Equals("max_pool_with_argmax") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("kH").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("kW").setIsArray(False).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("sH").setIsArray(False).setArgIndex(2).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("sW").setIsArray(False).setArgIndex(3).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("pH").setIsArray(False).setArgIndex(4).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("pW").setIsArray(False).setArgIndex(5).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("dH").setIsArray(False).setArgIndex(6).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("dW").setIsArray(False).setArgIndex(7).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("sameMode").setIsArray(False).setArgIndex(8).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("extraParam0").setIsArray(False).setArgIndex(9).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("isNHWC").setIsArray(False).setArgIndex(10).build()).build())
							End If
							If name.Equals("reshape") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("shapeArr").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("shape").setIsArray(False).setArgIndex(1).build()).build())
							End If
							If name.Equals("lin_space") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("dataType").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DATA_TYPE).setName("dataType").setIsArray(False).setArgIndex(0).build()).build())
							End If
							If name.Equals("create") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DATA_TYPE).setName("outputType").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("order").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("outputType").setIsArray(False).setArgIndex(1).build()).build())
							End If
							If name.Equals("extract_image_patches") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("isSameMode").setIsArray(False).setArgIndex(6).build()).build())
							End If
							If name.Equals("eye") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("numRows").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("numRows").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("numCols").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("numCols").setIsArray(False).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("batchDimension").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("batchDimension").setIsArray(True).setArgIndex(2).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("dataType").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("dataType").setIsArray(False).setArgIndex(3).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DOUBLE).setName("dataType").setIsArray(True).setArgIndex(0).build()).build())
							End If
							If name.Equals("range") Then
								Dim finalArgDescriptorProposals As IList(Of ArgDescriptorProposal) = argDescriptorProposals
								java.util.Arrays.asList(OpNamespace.ArgDescriptor.ArgType.DOUBLE, OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR, OpNamespace.ArgDescriptor.ArgType.INT64).forEach(Sub(dataType)
									finalArgDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(dataType).setName("from").setIsArray(False).setArgIndex(0).build()).build())
									finalArgDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(dataType).setName("to").setIsArray(False).setArgIndex(1).build()).build())
									finalArgDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(dataType).setName("step").setIsArray(True).setArgIndex(2).build()).build())
								End Sub)
							End If
							If name.Equals("onehot") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("input").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("input").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("axis").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("depth").setIsArray(False).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("on").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("on").setIsArray(False).setArgIndex(2).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("off").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("off").setIsArray(True).setArgIndex(3).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("axis").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("axis").setIsArray(True).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("depth").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("depth").setIsArray(True).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("on").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DOUBLE).setName("on").setIsArray(True).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("off").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DOUBLE).setName("off").setIsArray(True).setArgIndex(1).build()).build())
							End If
							If name.Equals("non_max_suppression") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("maxOutputSize").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("maxOutputSize").setIsArray(False).setArgIndex(2).build()).build())
							End If
							If name.Equals("pad") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("mode").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("mode").setIsArray(False).setArgIndex(0).build()).build())
							End If
							If name.Equals("range") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("l").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("l").setIsArray(False).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("l").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DOUBLE).setName("l").setIsArray(False).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("l").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("l").setIsArray(False).setArgIndex(1).build()).build())
							End If
							If name.Equals("repeat") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("dimensions").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("dimensions").setIsArray(False).setArgIndex(0).build()).build())
							End If
							If name.Equals("decode_bitmap") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("start").setIsArray(False).setArgIndex(0).build()).build())
							End If
							If name.Equals("dilation2d") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("isSameMode").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("rates").setIsArray(True).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("strides").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("strides").setIsArray(True).setArgIndex(2).build()).build())
							End If
							If name.Equals("standardize_bp") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("dimensions").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("dimensions").setIsArray(False).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("eps").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("eps").setIsArray(False).setArgIndex(2).build()).build())
							End If
							If name.Contains("fill") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("shape").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("result").setIsArray(False).setArgIndex(1).build()).build())
							End If
							If name.Contains("unsorted_") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("c++").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("input").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("c++").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("idxSegments").setIsArray(False).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("c++").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("numSegments").setIsArray(False).setArgIndex(2).build()).build())
							End If
							If name.Equals("lin_space") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("start").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("start").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("finish").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("finish").setIsArray(False).setArgIndex(1).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("numOfElements").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("numOfElements").setIsArray(False).setArgIndex(2).build()).build())
							End If
							If name.Equals("embedding_lookup") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("input").proposalWeight(9999999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("input").setIsArray(False).setArgIndex(0).build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("indices").proposalWeight(9999999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("indices").setIsArray(False).setArgIndex(1).build()).build())
							End If
							ret(name) = argDescriptorProposals
							Dim nIn As Integer = Integer.Parse(split(1).Trim())
							Dim nOut As Integer = Integer.Parse(split(2).Trim())
							currentOpNin = nIn
							currentOpNout = nOut
							hasNin = True
							hasNout = True
							Dim inplaceAble As Boolean = Boolean.Parse(split(3).Trim())
							Dim tArgs As Integer = Integer.Parse(split(4).Trim())
							Dim iArgs As Integer = Integer.Parse(split(5).Trim())
							currentOpIntArgs = iArgs
							currentOutTArgs = tArgs
							hasIntArgs = True
							hasTArgs = True
							builder.name(name).opDeclarationType(OpDeclarationDescriptor.OpDeclarationType.CUSTOM_OP_IMPL).nIn(nIn).nOut(nOut).inplaceAble(inplaceAble).iArgs(iArgs).tArgs(tArgs)
							inOpBlock = True
						ElseIf line.Contains(BOOLEAN_OP_IMPL) Then
							foundOp = True
							If line.Contains(");") Then
								oneLineOp = True
							End If
							line = removeBracesFromDeclarationMacro(line, BOOLEAN_OP_IMPL)
							Dim split() As String = line.Trim().Split(",", True)
							name = split(0)
							opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.BOOLEAN_OP_IMPL
							Dim nIn As Integer = Integer.Parse(split(1).Trim())
							currentOpNin = nIn
							hasNin = True
							Dim inplaceAble As Boolean = Boolean.Parse(split(2).Trim())
							builder.name(name).opDeclarationType(OpDeclarationDescriptor.OpDeclarationType.BOOLEAN_OP_IMPL).nIn(nIn).inplaceAble(inplaceAble)
							inOpBlock = True
						ElseIf line.Contains(LIST_OP_IMPL) Then
							foundOp = True
							line = removeBracesFromDeclarationMacro(line, LIST_OP_IMPL)
							Dim split() As String = line.Trim().Split(",", True)
							name = split(0)
							opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.LIST_OP_IMPL
							argDescriptorProposals = New List(Of ArgDescriptorProposal)()
							ret(name) = argDescriptorProposals
							Dim nIn As Integer = Integer.Parse(split(1).Trim())
							Dim nOut As Integer = Integer.Parse(split(2).Trim())
							currentOpNin = nIn
							currentOpNout = nOut
							hasNin = True
							hasNout = True
							Dim tArgs As Integer = Integer.Parse(split(3).Trim())
							Dim iArgs As Integer = Integer.Parse(split(4).Trim())
							currentOpIntArgs = iArgs
							currentOutTArgs = tArgs
							hasIntArgs = True
							hasTArgs = True
							builder.name(name).opDeclarationType(OpDeclarationDescriptor.OpDeclarationType.LIST_OP_IMPL).nIn(nIn).nOut(nOut).iArgs(iArgs).tArgs(tArgs)
							inOpBlock = True
							If name.Equals("split_list") OrElse name.Equals("scatter_list") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("list").build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(1).setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("array").build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(2).setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("sizes").build()).build())
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DATA_TYPE).setName("dtype").setIsArray(False).setArgIndex(0).build()).build())
							End If
							If name.Equals("read_list") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DATA_TYPE).setName("importDataType").setIsArray(False).setArgIndex(0).build()).build())
							End If
							If name.Equals("gather_list") OrElse name.Equals("stack_list") OrElse name.Equals("split_list") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.PositiveInfinity).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DATA_TYPE).setName("dtype").setIsArray(False).setArgIndex(0).build()).build())
							End If
						ElseIf line.Contains(LOGIC_OP_IMPL) Then
							foundOp = True
							If line.Contains(");") Then
								oneLineOp = True
							End If
							line = removeBracesFromDeclarationMacro(line, LOGIC_OP_IMPL)
							Dim split() As String = line.Trim().Split(",", True)
							name = split(0)
							opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.LOGIC_OP_IMPL
							argDescriptorProposals = New List(Of ArgDescriptorProposal)()
							ret(name) = argDescriptorProposals
							builder.name(name).opDeclarationType(OpDeclarationDescriptor.OpDeclarationType.LOGIC_OP_IMPL)
							inOpBlock = True
							If name.Equals("While") OrElse name.Equals("Switch") Or name.Equals("Conditional") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setArgType(OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR).setName("output").build()).build())
							End If
							If name.Equals("merge") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(99999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setIsArray(True).setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("inputs").build()).build())
							End If
							If name.Equals("While") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("condition").build()).build())
							End If
							If name.Equals("while") OrElse name.Equals("enter") OrElse name.Equals("exit") OrElse name.Equals("next_iteration") OrElse name.Equals("loop_cond") OrElse name.Equals("switch") OrElse name.Equals("While") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setArgType(OpNamespace.ArgDescriptor.ArgType.STRING).setName("frameName").build()).build())
							End If
						ElseIf line.Contains(DIVERGENT_OP_IMPL) Then
							foundOp = True
							line = removeBracesFromDeclarationMacro(line, DIVERGENT_OP_IMPL)
							Dim split() As String = line.Trim().Split(",", True)
							name = split(0)
							opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.DIVERGENT_OP_IMPL
							argDescriptorProposals = New List(Of ArgDescriptorProposal)()
							ret(name) = argDescriptorProposals
							Dim nIn As Integer = Integer.Parse(split(1).Trim())
							Dim nOut As Integer = Integer.Parse(split(2).Trim())
							currentOpNin = nIn
							currentOpNout = nOut
							hasNin = True
							hasNout = True
							Dim inplaceAble As Boolean = Boolean.Parse(split(3).Trim())
							builder.name(name).opDeclarationType(OpDeclarationDescriptor.OpDeclarationType.DIVERGENT_OP_IMPL).nIn(nIn).nOut(nOut).inplaceAble(inplaceAble)
							inOpBlock = True
						ElseIf line.Contains(CONFIGURABLE_OP_IMPL) Then
							foundOp = True
							line = removeBracesFromDeclarationMacro(line, CONFIGURABLE_OP_IMPL)
							Dim split() As String = line.Trim().Split(",", True)
							name = split(0)
							opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.CONFIGURABLE_OP_IMPL
							argDescriptorProposals = New List(Of ArgDescriptorProposal)()
							ret(name) = argDescriptorProposals
							Dim nIn As Integer = Integer.Parse(split(1).Trim())
							Dim nOut As Integer = Integer.Parse(split(2).Trim())
							currentOpNin = nIn
							currentOpNout = nOut
							hasNin = True
							hasNout = True
							Dim inplaceAble As Boolean = Boolean.Parse(split(3).Trim())
							Dim tArgs As Integer = Integer.Parse(split(4).Trim())
							Dim iArgs As Integer = Integer.Parse(split(5).Trim())
							hasIntArgs = True
							hasTArgs = True
							builder.name(name).opDeclarationType(OpDeclarationDescriptor.OpDeclarationType.CONFIGURABLE_OP_IMPL).nIn(nIn).nOut(nOut).inplaceAble(inplaceAble).iArgs(iArgs).tArgs(tArgs)
							inOpBlock = True
							If name.Equals("relu6") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setArgType(OpNamespace.ArgDescriptor.ArgType.DATA_TYPE).setName("dtype").build()).sourceOfProposal("cpp").proposalWeight(999999.0).build())
							End If
							If name.Contains("scatter_update") Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("cpp").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("indices").setIsArray(False).setArgIndex(2).build()).build())
							End If
						ElseIf line.Contains(REDUCTION_OP_IMPL) Then
							foundOp = True
							line = removeBracesFromDeclarationMacro(line, REDUCTION_OP_IMPL)
							Dim split() As String = line.Trim().Split(",", True)
							name = split(0)
							opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.REDUCTION_OP_IMPL
							argDescriptorProposals = New List(Of ArgDescriptorProposal)()
							ret(name) = argDescriptorProposals
							Dim nIn As Integer = Integer.Parse(split(1).Trim())
							Dim nOut As Integer = Integer.Parse(split(2).Trim())
							currentOpNin = nIn
							currentOpNout = nOut
							hasNin = True
							hasNout = True
							Dim inplaceAble As Boolean = Boolean.Parse(split(3).Trim())
							Dim tArgs As Integer = Integer.Parse(split(4).Trim())
							Dim iArgs As Integer = Integer.Parse(split(5).Trim())
							hasIntArgs = True
							hasTArgs = True
							builder.name(name).opDeclarationType(OpDeclarationDescriptor.OpDeclarationType.REDUCTION_OP_IMPL).nIn(nIn).nOut(nOut).inplaceAble(inplaceAble).iArgs(iArgs).tArgs(tArgs)
							inOpBlock = True
						ElseIf line.Contains(BROADCASTABLE_OP_IMPL) Then
							foundOp = True
							line = removeBracesFromDeclarationMacro(line, BROADCASTABLE_OP_IMPL)
							Dim split() As String = line.Trim().Split(",", True)
							name = split(0)
							opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.BROADCASTABLE_OP_IMPL
							argDescriptorProposals = New List(Of ArgDescriptorProposal)()
							ret(name) = argDescriptorProposals
							Dim tArgs As Integer = Integer.Parse(split(1).Trim())
							Dim iArgs As Integer = Integer.Parse(split(2).Trim())
							hasTArgs = True
							hasIntArgs = True
							builder.name(name).opDeclarationType(OpDeclarationDescriptor.OpDeclarationType.BROADCASTABLE_OP_IMPL).nIn(BROADCASTABLE_OP_IMPL_DEFAULT_NIN).nOut(BROADCASTABLE_OP_IMPL_DEFAULT_NOUT).iArgs(iArgs).tArgs(tArgs)
							inOpBlock = True
						ElseIf line.Contains(BROADCASTABLE_BOOL_OP_IMPL) Then
							foundOp = True
							line = line.Replace(BROADCASTABLE_BOOL_OP_IMPL & "(", "")
							line = line.Replace(")", "")
							line = line.Replace("{", "")
							Dim split() As String = line.Trim().Split(",", True)
							name = split(0)
							opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.BROADCASTABLE_BOOL_OP_IMPL
							argDescriptorProposals = New List(Of ArgDescriptorProposal)()
							ret(name) = argDescriptorProposals
							Dim tArgs As Integer = Integer.Parse(split(1).Trim())
							Dim iArgs As Integer = Integer.Parse(split(2).Trim())
							currentOpIntArgs = iArgs
							currentOutTArgs = tArgs
							hasIntArgs = True
							hasTArgs = True
							builder.name(name).opDeclarationType(OpDeclarationDescriptor.OpDeclarationType.BROADCASTABLE_BOOL_OP_IMPL).nIn(BROADCASTABLE_OP_IMPL_DEFAULT_NIN).nOut(BROADCASTABLE_OP_IMPL_DEFAULT_NOUT).iArgs(iArgs).tArgs(tArgs)
							inOpBlock = True
						ElseIf line.Contains(PLATFORM_IMPL) Then
							foundOp = True
							line = removeBracesFromDeclarationMacro(line, PLATFORM_IMPL)
							Dim split() As String = line.Trim().Split(",", True)
							name = split(0)
							If name IsNot Nothing AndAlso Not opTypes.ContainsKey(name) Then
								opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.PLATFORM_IMPL
							End If
							builder.name(name).opDeclarationType(OpDeclarationDescriptor.OpDeclarationType.PLATFORM_IMPL)
							inOpBlock = True
							hasNin = True
							hasNout = True
							platformImpl = True
						ElseIf line.Contains(OP_IMPL) Then
							foundOp = True
							line = removeBracesFromDeclarationMacro(line, OP_IMPL)
							Dim split() As String = line.Trim().Split(",", True)
							name = split(0)
							opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.OP_IMPL
							argDescriptorProposals = New List(Of ArgDescriptorProposal)()
							ret(name) = argDescriptorProposals
							Dim nIn As Integer = Integer.Parse(split(1).Trim())
							Dim nOut As Integer = Integer.Parse(split(2).Trim())
							currentOpNin = nIn
							currentOpNout = nOut
							hasNin = True
							hasNout = True
							Dim inplaceAble As Boolean = Boolean.Parse(split(3).Trim())
							builder.name(name).opDeclarationType(OpDeclarationDescriptor.OpDeclarationType.OP_IMPL).nIn(nIn).nOut(nOut).inplaceAble(inplaceAble)
							inOpBlock = True
						End If
					End If
					line = line.Trim()
					If inOpBlock AndAlso line.Contains([RETURN]) AndAlso endOfBlock(currLineIdx,lines) OrElse oneLineOp Then
						If foundOp Then
							If outArgNames.Count = 0 Then
								outArgNames.Add("output")
								outArgIndices.Add(0)
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setArgType(OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR).setName("output").build()).sourceOfProposal("cpp").proposalWeight(999999.0).build())
							End If
							builder.inArgNames(inArgNames)
							builder.outArgNames(outArgNames)
							builder.tArgNames(tArgNames)
							builder.iArgNames(iArgNames)
							builder.bArgNames(bArgNames)
							opDeclarationDescriptor = builder.build()
							Console.WriteLine(opDeclarationDescriptor)
							opDeclarationDescriptors.Add(opDeclarationDescriptor)
							If opDeclarationDescriptor IsNot Nothing Then
								Console.WriteLine("Op descriptor " & opDeclarationDescriptor)
								Console.WriteLine("Input arg name " & inArgNames)
								Console.WriteLine("Output arg names " & outArgNames)
								Console.WriteLine("T Arg names " & tArgNames)
								Console.WriteLine("Integer arg names " & iArgNames)
								Console.WriteLine("Boolean arg names " & bArgNames)
								opDeclarationDescriptor.validate()
							End If
						End If
						descriptorMap(opDeclarationDescriptor.getName()) = opDeclarationDescriptor
						inOpBlock = False
						foundOp = False
						oneLineOp = False
						opDeclarationDescriptor = Nothing
						builder = OpDeclarationDescriptor.builder()
						inArgNames = New List(Of String)()
						outArgNames = New List(Of String)()
						tArgNames = New List(Of String)()
						iArgNames = New List(Of String)()
						bArgNames = New List(Of String)()
						iArgIndices = New List(Of Integer)()
						bArgIndices = New List(Of Integer)()
						inArgIndices = New List(Of Integer)()
						tArgIndices = New List(Of Integer)()
						outArgIndices = New List(Of Integer)()
						currentOpNin = -1
						currentOpNout = -1
						hasNin = False
						hasNout = False
						hasIntArgs = False
						hasTArgs = False
						currentOpBooleanArgs = -1
						currentOpIntArgs = -1
						currentOutTArgs = -1
						platformImpl = False
						argDescriptorProposals = New List(Of ArgDescriptorProposal)()
					End If
					If inOpBlock Then
						If argDescriptorProposals Is Nothing Then
							argDescriptorProposals = New List(Of ArgDescriptorProposal)()
						End If
						If line.Length = 0 Then
						End If
						If matchesArgDeclaration(INT_ARG,line) Then
							processLine(iArgNames, iArgIndices, argDescriptorProposals, line, OpNamespace.ArgDescriptor.ArgType.INT64,name)
							If name.Contains("maxpool2d") Then
								If Not containsProposalWithDescriptorName("extraParam0",argDescriptorProposals) Then
									argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("extraParam0").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("extraParam0").setIsArray(False).setArgIndex(9).build()).build())
								End If
							End If
							If name.Equals("top_k") Then
								If Not containsProposalWithDescriptorName("sorted",argDescriptorProposals) Then
									argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("sorted").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("sorted").setIsArray(False).setArgIndex(0).build()).build())
								End If
							End If
						End If
						If matchesArgDeclaration(OUTPUT_NULLIFIED,line) OrElse matchesArgDeclaration(OUTPUT_VARIABLE,line) AndAlso Not line.Contains("->rankOf()") Then
							processLine(outArgNames, outArgIndices, argDescriptorProposals, line, OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR,name)
						End If
						If matchesArgDeclaration(T_ARG,line) AndAlso Not line.Contains("INT") Then
							processLine(tArgNames, tArgIndices, argDescriptorProposals, line, OpNamespace.ArgDescriptor.ArgType.DOUBLE, name)
						End If
						If Not line.Contains("->rankOf()") AndAlso Not line.Contains("->dataType()") AndAlso matchesArgDeclaration(INPUT_VARIABLE,line) OrElse matchesArgDeclaration(INPUT_LIST,line) Then
							processLine(inArgNames,inArgIndices,argDescriptorProposals,line, OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR, name)
						End If
						If matchesArgDeclaration(B_ARG,line) Then
							processLine(bArgNames, bArgIndices, argDescriptorProposals, line, OpNamespace.ArgDescriptor.ArgType.BOOL,name)
						End If
						If matchesArrayArgDeclaration(line.Trim()) Then
							If line.Contains(INT_ARG) Then
								processArrayLine(iArgNames, iArgIndices, argDescriptorProposals, line, OpNamespace.ArgDescriptor.ArgType.INT64)
							End If
							If line.Contains(OUTPUT_NULLIFIED) OrElse line.Contains(OUTPUT_VARIABLE) Then
								processArrayLine(outArgNames, outArgIndices, argDescriptorProposals, line, OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR)
							End If
							If line.Contains(T_ARG) AndAlso Not line.Contains("INT") Then
								processArrayLine(tArgNames, tArgIndices, argDescriptorProposals, line, OpNamespace.ArgDescriptor.ArgType.DOUBLE)
							End If
							If line.Contains(B_ARG) Then
								processArrayLine(bArgNames, bArgIndices, argDescriptorProposals, line, OpNamespace.ArgDescriptor.ArgType.BOOL)
							End If
						End If
					End If
					If line.Contains(DECLARE_SYN) Then
						line = removeBracesFromDeclarationMacro(line, DECLARE_SYN)
						Dim args2() As String = line.Split(",", True)
						Dim aliasFor As String = args2(1).Trim()
						Dim newKey As String = args2(0).Trim()
						If descriptorMap.Count = 0 Then
							Throw New System.InvalidOperationException("Descriptor map should not be empty here")
						End If
						Dim opDescriptor2 As OpDeclarationDescriptor.OpDeclarationDescriptorBuilder = descriptorMap(aliasFor).toBuilder()
						opDescriptor2.name(newKey)
						Dim newDescriptor As OpDeclarationDescriptor = opDescriptor2.build()
						opDeclarationDescriptors.Add(newDescriptor)
						descriptorMap(args2(1)) = newDescriptor
					End If
					currLineIdx += 1
				Next line
			Catch e As IOException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try
			End Sub)



			Return ret

		End Function

		Private Function endOfBlock(ByVal lineIndex As Integer, ByVal lines As IList(Of String)) As Boolean
			If lineIndex < lines.Count - 2 Then
				For i As Integer = lineIndex To lines.Count - 3
					'could be last brace
					If lines(i + 1).Trim().Equals("}") OrElse lines(i + 1).Trim().Equals("};") OrElse lines(i + 1).Length = 0 OrElse lines(i + 1).Trim().Length = 0 Then
						Continue For
					End If
					If lines(i + 1).Contains("DECLARE_TYPES") OrElse lines(i + 1).Contains("DECLARE_SHAPE_FN") OrElse lines(i + 1).Contains("DECLARE_SYN") OrElse lines(i).Contains("DECLARE_TYPES") OrElse lines(i).Contains("DECLARE_SHAPE_FN") OrElse lines(i).Contains("DECLARE_SYN") OrElse lines(i + 1).Contains("OP_") OrElse lines(i + 1).Contains("////") Then
						Return True
					ElseIf Not lines(i + 1).Contains("DECLARE_TYPES") OrElse Not lines(i + 1).Contains("DECLARE_SHAPE_FN") OrElse Not lines(i + 1).Contains("DECLARE_SYN") OrElse Not lines(i + 1).Contains("OP_") OrElse Not lines(i + 1).Contains("////") Then
						Return False
					End If
				Next i
			End If

			Return True

		End Function

		Private Function argDeclarationForType(ByVal argType As OpNamespace.ArgDescriptor.ArgType) As String
			Select Case argType.innerEnumValue
				Case OpNamespace.ArgDescriptor.ArgType.InnerEnum.INPUT_TENSOR
					Return INPUT_VARIABLE
				Case OpNamespace.ArgDescriptor.ArgType.InnerEnum.INT32, INT64
					Return INT_ARG
				Case OpNamespace.ArgDescriptor.ArgType.InnerEnum.FLOAT, [DOUBLE]
					Return T_ARG
				Case OpNamespace.ArgDescriptor.ArgType.InnerEnum.BOOL
					Return B_ARG
				Case OpNamespace.ArgDescriptor.ArgType.InnerEnum.OUTPUT_TENSOR
					Return OUTPUT_VARIABLE
				Case Else
					Throw New System.ArgumentException("Processing illegal type " & argType)

			End Select
		End Function


		Private Sub processArrayLine(ByVal iArgNames As IList(Of String), ByVal iArgIndices As IList(Of Integer), ByVal argDescriptorProposals As IList(Of ArgDescriptorProposal), ByVal line As String, ByVal argType As OpNamespace.ArgDescriptor.ArgType)
			Dim split() As String = line.Split(" = ", True)
			If split.Length = 1 Then
				'invalid line
				Return
			End If

			Dim arrSplit() As String = split(0).Split(" ", True)
			Dim name As String = arrSplit(0).replaceAll("\[.*\]","")
			Preconditions.checkState(name.Length > 0)
			addArrayNameToList(line, iArgNames, iArgIndices, argDeclarationForType(argType))


			Dim argDescriptor As OpNamespace.ArgDescriptor = OpNamespace.ArgDescriptor.newBuilder().setArgType(argType).setIsArray(True).setConvertBoolToInt(argType = OpNamespace.ArgDescriptor.ArgType.BOOL OrElse line.Contains("B_ARG")).setName(name).setArgIndex(-1).build()

			Dim weightToIncrementBy As Double = weight * 1000000
			Dim argDescriptorProposal As ArgDescriptorProposal = ArgDescriptorProposal.builder().descriptor(argDescriptor).sourceLine(line).sourceOfProposal("cpp").proposalWeight(weightToIncrementBy).build()
			argDescriptorProposals.Add(argDescriptorProposal)
		End Sub


		Private Sub processLine(ByVal iArgNames As IList(Of String), ByVal iArgIndices As IList(Of Integer), ByVal argDescriptorProposals As IList(Of ArgDescriptorProposal), ByVal line As String, ByVal argType As OpNamespace.ArgDescriptor.ArgType, ByVal opName As String)
			Dim matchesPureDeclaration As Boolean = Pattern.matches(ARG_DECLARATION,line) OrElse Pattern.matches(ARG_BOOL_EQUALS_DECLARATION,line) OrElse Pattern.matches(ARRAY_ASSIGNMENT,line)
			Dim split() As String = line.Split("\s*=\s*", True)
			If split.Length = 1 Then
				'invalid line
				Return
			End If

			Dim arrSplit() As String = split(0).Split(" ", True)
			'type + name
			Dim index As Integer? = extractArgFromCpp(line, argDeclarationForType(argType))
			'guess index based on current number of indices already added
			If index < 0 Then
				index = iArgIndices.Count
			End If


			addNameToList(line, iArgNames, iArgIndices, argDeclarationForType(argType))
			'note sometimes we have individual array entries for names, we need to strip out index indicators like [i]
			Dim argName As String = arrSplit(arrSplit.Length - 1).replaceAll("\[.*\]","")
			If containsProposalWithDescriptorName(argName,argDescriptorProposals) Then
				Dim descriptor As val = getDescriptorWithName(argName,argDescriptorProposals)
				'don't add already encountered indices if one is already greater.
				If descriptor IsNot Nothing Then
					Return
				End If
			End If


			Preconditions.checkState(argName.Length > 0)
			'more than a typename variable name present
			If arrSplit.Length > 2 Then
				'skip type
				For i As Integer = 1 To arrSplit.Length - 1
					'handle inline comments
					arrSplit(i) = arrSplit(i).Trim()
					arrSplit(i) = arrSplit(i).Replace(";","")
					If isValidIdentifier(arrSplit(i)) Then
						argName = arrSplit(i)
						Preconditions.checkState(argName.Length > 0)
						Exit For
					End If
				Next i
			End If

			Preconditions.checkState(argName.Length > 0)

			Dim argDescriptor As OpNamespace.ArgDescriptor = OpNamespace.ArgDescriptor.newBuilder().setArgType(argType).setConvertBoolToInt(argType = OpNamespace.ArgDescriptor.ArgType.BOOL AndAlso Not line.Contains("B_ARG")).setName(argName).setArgIndex(index).build()
			Dim weightToIncrementBy As Double = If(matchesPureDeclaration, weight * 1000000, weight)
			If line.Contains("->") Then
				weightToIncrementBy -= 100000
			End If

			Dim argDescriptorProposal As ArgDescriptorProposal = ArgDescriptorProposal.builder().descriptor(argDescriptor).sourceOfProposal("cpp").sourceLine(line).proposalWeight(weightToIncrementBy).build()
			argDescriptorProposals.Add(argDescriptorProposal)

			'remove duplicate proposals and only take the max index ensuring all parameters are accounted for
'JAVA TO VB CONVERTER TODO TASK: Most Java stream collectors are not converted by Java to VB Converter:
			Dim groupedByName As val = argDescriptorProposals.collect(Collectors.groupingBy(Function(proposal) proposal.getDescriptor().getName()))
			Dim toRemove As IList(Of ArgDescriptorProposal) = New List(Of ArgDescriptorProposal)()
			If Not bannedMaxIndexOps.Contains(opName) Then
				For Each proposals As KeyValuePair(Of String, IList(Of ArgDescriptorProposal)) In groupedByName.entrySet()
					If proposals.Value.size() > 1 Then
						Dim max As ArgDescriptorProposal = Nothing
						For Each proposal As ArgDescriptorProposal In proposals.Value
							If max Is Nothing Then
								max = proposal
							ElseIf max.getDescriptor().getArgIndex() < proposal.getDescriptor().getArgIndex() Then
								'slate for removal and set new max
								toRemove.Add(max)
								max = proposal
							End If
						Next proposal

					End If
				Next proposals
			End If

			argDescriptorProposals.RemoveAll(toRemove)

		End Sub

		Public Overridable ReadOnly Property Proposals As IDictionary(Of String, IList(Of ArgDescriptorProposal))
			Get
				Return doExtractArgDescriptors()
			End Get
		End Property

		Public Overridable Function typeFor(ByVal name As String) As OpNamespace.OpDescriptor.OpDeclarationType Implements ArgDescriptorSource.typeFor
			Return opTypes(name)
		End Function
	End Class

End Namespace