Imports System
Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Pooling3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling3DConfig
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops.impl.layers.convolution



	''' <summary>
	''' Pooling3D operation
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NoArgsConstructor public abstract class Pooling3D extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public MustInherit Class Pooling3D
		Inherits DynamicCustomOp

		Protected Friend config As Pooling3DConfig

		Public Enum Pooling3DType
			MAX
			AVG
			PNORM
		End Enum

		Public Overrides Function iArgs() As Long()
			If iArguments.Count = 0 Then
				addArgs()
			End If

			Return MyBase.iArgs()
		End Function

		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable, ByVal inputArrays() As INDArray, ByVal outputs() As INDArray, ByVal inPlace As Boolean, ByVal pooling3DConfig As Pooling3DConfig, ByVal type As Pooling3DType)
			MyBase.New(Nothing,sameDiff, inputs, inPlace)
			Preconditions.checkState(pooling3DConfig.getDD() > 0 AndAlso pooling3DConfig.getDH() > 0 AndAlso pooling3DConfig.getDW() > 0, "Dilation values must all be > 0: got dD/H/W = %s/%s/%s", pooling3DConfig.getDD(), pooling3DConfig.getDH(), pooling3DConfig.getDW())

			If type <> Nothing Then
				pooling3DConfig.setType(type)
			End If

			Me.config = pooling3DConfig
			Me.sameDiff = sameDiff

			If inputArrays IsNot Nothing Then
				addInputArgument(inputArrays)
			End If
			If outputs IsNot Nothing Then
				addOutputArgument(outputs)
			End If
			addArgs()
		End Sub

		Public Overrides ReadOnly Property ConfigProperties As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function configFieldName() As String
			Return "config"
		End Function

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			Return config.toProperties()
		End Function

		Protected Friend Overridable Sub addArgs()
			If Me.iArguments Is Nothing Then
				Me.iArguments = New List(Of Long)()
			End If
			addIArgument(config.getKD())
			addIArgument(config.getKW())
			addIArgument(config.getKH())
			addIArgument(config.getSD())
			addIArgument(config.getSW())
			addIArgument(config.getSH())
			addIArgument(config.getPD())
			addIArgument(config.getPW())
			addIArgument(config.getPH())
			addIArgument(config.getDD())
			addIArgument(config.getDW())
			addIArgument(config.getDH())
			addIArgument(If(config.isSameMode(), 1, 0)) 'Ceiling mode == same mode
			addIArgument(0) '0 == "exclude padding from average count"
			addIArgument(If(config.isNCDHW(), 0, 1))

		End Sub

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As IList(Of SDVariable) = New List(Of SDVariable)()
			Dim inputs As IList(Of SDVariable) = New List(Of SDVariable)()
			CType(inputs, List(Of SDVariable)).AddRange(New List(Of SDVariable) From {args()})
			inputs.Add(f1(0))
			Dim pooling3DDerivative As Pooling3DDerivative = Pooling3DDerivative.derivativeBuilder().inPlace(inPlace).sameDiff(sameDiff).inputs(CType(inputs, List(Of SDVariable)).ToArray()).pooling3DConfig(config).build()
			CType(ret, List(Of SDVariable)).AddRange(New List(Of SDVariable) From {pooling3DDerivative.outputVariables()})

			Return ret
		End Function

		Public Overridable ReadOnly Property PoolingPrefix As String
			Get
				If config Is Nothing Then
					Return "pooling3d"
				End If
    
				Select Case config.getType()
					Case AVG
						Return "avg"
					Case MAX
						Return "max"
					Case Else
						Throw New System.InvalidOperationException("No pooling type found.")
				End Select
			End Get
		End Property

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim aStrides As val = nodeDef.getAttrOrThrow("strides")
			Dim tfStrides As IList(Of Long) = aStrides.getList().getIList()
			Dim aKernels As val = nodeDef.getAttrOrThrow("ksize")
			Dim tfKernels As IList(Of Long) = aKernels.getList().getIList()
			Dim aPadding As val = nodeDef.getAttrOrThrow("padding")
			Dim tfPadding As IList(Of Long) = aPadding.getList().getIList()

			Dim paddingMode As String = aPadding.getS().toStringUtf8().replaceAll("""", "")

			Dim isSameMode As Boolean = paddingMode.Equals("SAME", StringComparison.OrdinalIgnoreCase)

			Dim data_format As String = "ndhwc"
			If nodeDef.containsAttr("data_format") Then
				Dim attr As val = nodeDef.getAttrOrThrow("data_format")

				data_format = attr.getS().toStringUtf8().ToLower()
			End If

			'Order: depth, height, width
			'TF doesn't have dilation, it seems?
			Dim strides(2) As Integer
			Dim padding(2) As Integer
			Dim kernel(2) As Integer
			For i As Integer = 0 To 2
				'TF values here have 5 values: minibatch and Channels at positions 0 and 4, which are almost always 1
				strides(i) = tfStrides(i + 1).intValue()
				If tfPadding IsNot Nothing AndAlso tfPadding.Count > 0 Then
					'Empty for SAME mode
					padding(i) = tfPadding(i + 1).intValue()
				End If

				kernel(i) = tfKernels(i + 1).intValue()
			Next i

			Dim type As Pooling3DType
			Dim name As String = nodeDef.getOp().ToLower()
			If name.StartsWith("max", StringComparison.Ordinal) Then
				type = Pooling3DType.MAX
			ElseIf name.StartsWith("av", StringComparison.Ordinal) Then
				type = Pooling3DType.AVG
			Else
				Throw New System.InvalidOperationException("Unknown or not supported pooling type: " & name)
			End If

			Dim conf As Pooling3DConfig = Pooling3DConfig.builder().sD(strides(0)).sH(strides(1)).sW(strides(2)).pD(padding(0)).pH(padding(1)).pW(padding(2)).kD(kernel(0)).kH(kernel(1)).kW(kernel(2)).type(type).isSameMode(isSameMode).isNCDHW(data_format.Equals("ncdhw", StringComparison.OrdinalIgnoreCase)).build()
			Me.config = conf
			addArgs()
		End Sub

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for op " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
		  Throw New NoOpNameFoundException("No op opName found for op " & opName())
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input data type for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function

	End Class

End Namespace