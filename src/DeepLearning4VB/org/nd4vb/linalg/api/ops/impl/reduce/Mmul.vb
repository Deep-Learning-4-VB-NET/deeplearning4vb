Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports MMulTranspose = DeepLearning4VB.org.nd4vb.linalg.api.blas.params.MMulTranspose
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.reduce

    ''' <summary>The custom op <c>matmul</c> (Java <c>org.nd4j.linalg.api.ops.impl.reduce.Mmul</c>).</summary>
    Public Class Mmul
        Inherits DynamicCustomOp

        Protected Friend mt As MMulTranspose
        Protected Friend alpha As Double = 1.0
        Protected Friend beta As Double = 0.0

#If SAMEDIFF Then
		''' 
		''' <param name="sameDiff"> </param>
		''' <param name="i_v1"> </param>
		''' <param name="i_v2"> </param>
		''' <param name="mt"> </param>
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal mt As MMulTranspose)
			MyBase.New(Nothing,sameDiff,New SDVariable(){i_v1, i_v2})
			Me.mt = mt
			addIArgument(ArrayUtil.fromBoolean(mt.isTransposeA()), ArrayUtil.fromBoolean(mt.isTransposeB()), ArrayUtil.fromBoolean(mt.isTransposeResult()))
			addTArgument(alpha, beta)
		End Sub

		''' 
		''' <param name="sameDiff"> </param>
		''' <param name="i_v1"> </param>
		''' <param name="i_v2"> </param>
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable)
			Me.New(sameDiff,i_v1,i_v2,MMulTranspose.allFalse())
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal alpha As Double, ByVal beta As Double, ByVal mt As MMulTranspose)
            addInputArgument(x, y)

            If z IsNot Nothing Then
                addOutputArgument(z)
            End If

            If mt IsNot Nothing Then
                Me.mt = mt
                addIArgument(ArrayUtil.fromBoolean(mt.isTransposeA()), ArrayUtil.fromBoolean(mt.isTransposeB()), ArrayUtil.fromBoolean(mt.isTransposeResult()))
            End If

            Me.alpha = alpha
            Me.beta = beta

            addTArgument(alpha, beta)
        End Sub

        ''' 
        ''' <param name="x"> </param>
        ''' <param name="y"> </param>
        ''' <param name="z"> </param>
        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal mt As MMulTranspose)
            Me.New(x, y, z, 1.0, 0.0, mt)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal transposeX As Boolean, ByVal transposeY As Boolean, ByVal transposeZ As Boolean)
            Me.New(x, y, 1.0, 0.0, transposeX, transposeY, transposeZ)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal alpha As Double, ByVal beta As Double, ByVal transposeX As Boolean, ByVal transposeY As Boolean, ByVal transposeZ As Boolean)
            addInputArgument(x, y)
            addIArgument(ArrayUtil.fromBoolean(transposeX), ArrayUtil.fromBoolean(transposeY), ArrayUtil.fromBoolean(transposeZ))
            mt = MMulTranspose.builder().transposeA(transposeX).transposeB(transposeY).transposeResult(transposeZ).build()
            addTArgument(alpha, beta)
            Me.alpha = alpha
            Me.beta = beta
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal alpha As Double, ByVal beta As Double)
            Me.New(x,y,Nothing, alpha, beta,Nothing)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
            Me.New(x, y, 1.0, 0.0)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal transposeX As Boolean, ByVal transposeY As Boolean, ByVal transposeZ As Boolean)
			MyBase.New(Nothing,sameDiff,New SDVariable(){x, y})
			addIArgument(ArrayUtil.fromBoolean(transposeX), ArrayUtil.fromBoolean(transposeY), ArrayUtil.fromBoolean(transposeZ))

			addTArgument(alpha, beta)
			mt = MMulTranspose.builder().transposeA(transposeX).transposeB(transposeY).transposeResult(transposeZ).build()
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Overrides Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
            If mt Is Nothing Then
                mt = MMulTranspose.builder().build()
            End If

            Return mt.getValue([property])
        End Function

        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
            If mt Is Nothing Then
                Return New Dictionary(Of String, Object)()
            End If
            Return mt.toProperties()
        End Function

        Public Overrides ReadOnly Property ConfigProperties As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides Function configFieldName() As String
            Return "mt"
        End Function

        Public Overrides Sub setPropertiesForFunction(ByVal properties As IDictionary(Of String, Object))
            If mt Is Nothing Then
                mt = MMulTranspose.builder().build()
            End If
            mt.setProperties(properties)
        End Sub

        ''' <summary>
        ''' For a 2D matrix of shape (M, N) we return (N, M).
        ''' For a 3D matrix with leading mini-batch dimension (mb, M, N)
        ''' we return (mb, N, M)
        ''' </summary>
        ''' <param name="shape"> input shape array
        ''' @return </param>
        Public Overridable Function transposeShapeArray(ByVal shape() As Long) As Long()
            If shape.Length = 2 Then
                Return ArrayUtil.reverseCopy(shape)
            ElseIf shape.Length = 3 Then
                Return New Long() {shape(0), shape(2), shape(1)}
            Else
                Throw New System.ArgumentException("Matrix input has to be of length 2 or 3, got: " & shape.Length)
            End If

        End Function

        Public Overrides Function onnxName() As String
            Return "MatMul"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"MatMul", "BatchMatMul", "BatchMatMulV2"}
        End Function

        Public Overrides Function opName() As String
            Return "matmul"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)

			Dim isTransposeA As Boolean
			Dim isTransposeB As Boolean
			If nodeDef.getOp().equalsIgnoreCase("MatMul") Then
				isTransposeA = attributesForNode("transpose_a").getB()
				isTransposeB = attributesForNode("transpose_b").getB()

			Else
				'BatchMatMul, BatchMatMulV2
				'In practice, BatchMatMul seems to use "adj_x" and "adj_y" instead of "transpose_a" and "transpose_b"
				If attributesForNode.ContainsKey("transpose_a") Then
					isTransposeA = attributesForNode("transpose_a").getB()
				Else
					isTransposeA = attributesForNode("adj_x").getB()
				End If
				If attributesForNode.ContainsKey("transpose_b") Then
					isTransposeB = attributesForNode("transpose_b").getB()
				Else
					isTransposeB = attributesForNode("adj_y").getB()
				End If
			End If
			Dim mMulTranspose As MMulTranspose = MMulTranspose.builder().transposeA(isTransposeA).transposeB(isTransposeB).build()
			Me.mt = mMulTranspose
			iArguments.Clear()
			addIArgument(ArrayUtil.fromBoolean(mt.isTransposeA()), ArrayUtil.fromBoolean(mt.isTransposeB()))
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Dim isTransposeA As val = If(Not attributesForNode.ContainsKey("transA"), False, attributesForNode("transA").getI() > 0)
			Dim isTransposeB As val = If(Not attributesForNode.ContainsKey("transB"), False, attributesForNode("transB").getI() > 0)
			Dim mMulTranspose As MMulTranspose = MMulTranspose.builder().transposeA(isTransposeA).transposeB(isTransposeB).build()
			Me.mt = mMulTranspose
		End Sub
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {(New MmulBp(sameDiff, larg(), rarg(), gradients(0), mt)).outputVariables()}
		End Function
#End If

#If MODEL_IMPORT Then
		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim transposeA As val = PropertyMapping.builder().onnxAttrName("transA").tfAttrName("transpose_a").propertyNames(New String(){"transposeA"}).build()

			Dim transposeB As val = PropertyMapping.builder().onnxAttrName("transB").tfAttrName("transpose_b").propertyNames(New String(){"transposeB"}).build()

			map("transposeA") = transposeA
			map("transposeB") = transposeB

			For Each s As String In tensorflowNames()
				ret(s) = map
			Next s
			ret(onnxName()) = map

			Return ret
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
           If dArguments.Count > 0 Then
               Return New List(Of DataType) From {dArguments(0)}
           End If
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count >= 2, "Expected at least 2 inputs to mmul op, got %s", dataTypes)
            Preconditions.checkState(dataTypes(0).isFPType() AndAlso dataTypes(1).isFPType(), "Inputs to mmul op must both be a floating" & "point type: got %s", dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As Mmul = TryCast(o, Mmul)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(mt, other.mt) Then
                Return False
            End If
            If Not Object.Equals(alpha, other.alpha) Then
                Return False
            End If
            If Not Object.Equals(beta, other.beta) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(mt) Is Nothing, 43, mt.GetHashCode())
            result = result * 59 + If(CObj(alpha) Is Nothing, 43, alpha.GetHashCode())
            result = result * 59 + If(CObj(beta) Is Nothing, 43, beta.GetHashCode())
            Return result
        End Function

    End Class

End Namespace
