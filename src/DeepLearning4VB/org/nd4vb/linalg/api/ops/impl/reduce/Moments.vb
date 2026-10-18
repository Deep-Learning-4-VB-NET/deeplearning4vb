Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports MeanBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.bp.MeanBp
Imports VarianceBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.bp.VarianceBp
#End If
#If MODEL_IMPORT Then
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
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

    ''' <summary>The custom op <c>moments</c> (Java <c>org.nd4j.linalg.api.ops.impl.reduce.Moments</c>).</summary>
    Public Class Moments
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private axes() As Integer

        Public Sub New(ByVal input As INDArray, ParamArray ByVal axes() As Integer)
            MyBase.New(New INDArray(){input}, Nothing)
            Me.axes = axes
            addArgs()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable)
			Me.New(sameDiff, input, Nothing)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal axes() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input}, False)
			Me.axes = axes
			addArgs()
		End Sub
#End If

        Public Sub New(ByVal [in] As INDArray, ByVal outMean As INDArray, ByVal outStd As INDArray, ParamArray ByVal axes() As Integer)
            MyBase.New(Nothing, New INDArray(){[in]}, New INDArray(){outMean, outStd}, Nothing, axes)
        End Sub

        Private Sub addArgs()
            If axes IsNot Nothing Then
                For Each axis As Integer In axes
                    addIArgument(axis)
                Next axis
            End If
        End Sub

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "moments"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim dLdMean As SDVariable = grad(0)
			Dim dLdVar As SDVariable = grad(1) 'Note: non-bias-corrected variance
			Dim meanBp As SDVariable = (New MeanBp(sameDiff, arg(), dLdMean, False, axes)).outputVariable()
			Dim varBp As SDVariable = (New VarianceBp(sameDiff, arg(), dLdVar, False, False, axes)).outputVariable()
			Return Collections.singletonList(meanBp.add(varBp))
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected 1 datatype for %s, got %s", Me.GetType(), dataTypes)
            If dataTypes(0).isFPType() Then
                Return New List(Of DataType) From {dataTypes(0), dataTypes(0)}
            End If
            Return New List(Of DataType) From {Nd4vbT.defaultFloatingPointType(), Nd4vbT.defaultFloatingPointType()}
        End Function

    End Class

End Namespace
