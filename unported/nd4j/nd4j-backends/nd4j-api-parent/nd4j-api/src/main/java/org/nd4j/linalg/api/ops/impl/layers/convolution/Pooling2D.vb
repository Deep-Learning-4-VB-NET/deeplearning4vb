Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Pooling2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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
	''' Pooling2D operation
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Getter public class Pooling2D extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Pooling2D
		Inherits DynamicCustomOp

		Protected Friend config As Pooling2DConfig

		Public Enum Pooling2DType
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

		''' <summary>
		''' Divisor mode for average pooling only. 3 modes are supported:
		''' MODE_0:
		''' EXCLUDE_PADDING:
		''' INCLUDE_PADDING: Always do sum(window) / (kH*kW) even if padding is present.
		''' </summary>
		Public Enum Divisor
			EXCLUDE_PADDING
			INCLUDE_PADDING
		End Enum

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "sameDiffBuilder") @SuppressWarnings("Used in lombok") public Pooling2D(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputs, org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable, ByVal config As Pooling2DConfig)
			MyBase.New(Nothing, sameDiff, inputs, False)

			Me.config = config
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Pooling2D(@NonNull INDArray[] inputs, org.nd4j.linalg.api.ndarray.INDArray[] outputs, @NonNull Pooling2DConfig config)
		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal config As Pooling2DConfig)
			MyBase.New(inputs, outputs)

			Me.config = config
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Pooling2D(@NonNull INDArray input, org.nd4j.linalg.api.ndarray.INDArray output, @NonNull Pooling2DConfig config)
		Public Sub New(ByVal input As INDArray, ByVal output As INDArray, ByVal config As Pooling2DConfig)
			MyBase.New(New INDArray(){input}, wrapOrNull(output))

			Me.config = config
			addArgs()
		End Sub

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			Return config.toProperties()
		End Function

		Private Sub addArgs()
			Dim t As val = config.getType()

			addIArgument(config.getKH())
			addIArgument(config.getKW())
			addIArgument(config.getSH())
			addIArgument(config.getSW())
			addIArgument(config.getPH())
			addIArgument(config.getPW())
			addIArgument(config.getDH())
			addIArgument(config.getDW())
			addIArgument(ArrayUtil.fromBoolean(config.isSameMode()))
			addIArgument(If(t = Pooling2DType.AVG, config.getDivisor().ordinal(), CInt(Math.Truncate(config.getExtra()))))
			addIArgument(ArrayUtil.fromBoolean(config.isNHWC()))
		End Sub

		Public Overrides ReadOnly Property ConfigProperties As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function configFieldName() As String
			Return "config"
		End Function

		Public Overrides Function opName() As String
			Return PoolingPrefix & "pool2d"
		End Function


		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As IList(Of SDVariable) = New List(Of SDVariable)()
			Dim inputs As IList(Of SDVariable) = New List(Of SDVariable)()
			CType(inputs, List(Of SDVariable)).AddRange(New List(Of SDVariable) From {args()})
			inputs.Add(f1(0))
			Dim pooling2DDerivative As Pooling2DDerivative = Pooling2DDerivative.derivativeBuilder().inputs(CType(inputs, List(Of SDVariable)).ToArray()).sameDiff(sameDiff).config(config).build()
			CType(ret, List(Of SDVariable)).AddRange(New List(Of SDVariable) From {pooling2DDerivative.outputVariables()})
			Return ret
		End Function



		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim aStrides As val = nodeDef.getAttrOrThrow("strides")
			Dim tfStrides As val = aStrides.getList().getIList()
			Dim sH As val = tfStrides.get(1)
			Dim sW As val = tfStrides.get(2)

			Dim aKernels As val = nodeDef.getAttrOrThrow("ksize")
			Dim tfKernels As val = aKernels.getList().getIList()

			Dim kH As val = tfKernels.get(1)
			Dim kW As val = tfKernels.get(2)

			Dim aPadding As val = nodeDef.getAttrOrThrow("padding")
			Dim padding As val = aPadding.getList().getIList()

			Dim paddingMode As val = aPadding.getS().toStringUtf8().replaceAll("""","")

			Dim isSameMode As Boolean = paddingMode.equalsIgnoreCase("SAME")

			If Not isSameMode Then
				log.debug("Mode: {}", paddingMode)
			End If

			Dim pooling2DConfig As Pooling2DConfig = Pooling2DConfig.builder().sH(sH.intValue()).sW(sW.intValue()).type(Nothing).isSameMode(isSameMode).kH(kH.intValue()).kW(kW.intValue()).pH(padding.get(0).intValue()).pW(padding.get(1).intValue()).build()
			Me.config = pooling2DConfig
			addArgs()
			log.debug("Pooling: k: [{},{}]; s: [{}, {}], padding: {}", kH, kW, sH, sW, aPadding)


		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Dim isSameNode As val = attributesForNode("auto_pad").getS().Equals("SAME")
			Dim kernelShape As val = attributesForNode("kernel_shape").getIntsList()
			Dim padding As val = attributesForNode("pads").getIntsList()
			Dim strides As val = attributesForNode("strides").getIntsList()

			Dim pooling2DConfig As Pooling2DConfig = Pooling2DConfig.builder().sW(strides.get(0).intValue()).sH(strides.get(1).intValue()).type(Nothing).isSameMode(isSameNode).kH(kernelShape.get(0).intValue()).kW(kernelShape.get(1).intValue()).pH(padding.get(0).intValue()).pW(padding.get(1).intValue()).build()
			Me.config = pooling2DConfig
			addArgs()
		End Sub


		Public Overridable ReadOnly Property PoolingPrefix As String
			Get
				If config Is Nothing Then
					Return "somepooling"
				End If
    
				Select Case config.getType()
					Case AVG
						Return "avg"
					Case MAX
						Return "max"
					Case PNORM
						Return "pnorm"
					Case Else
						Throw New System.InvalidOperationException("No pooling type found.")
				End Select
			End Get
		End Property


		Public Overrides Function onnxName() As String
			Return "Pooling"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input data type for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace