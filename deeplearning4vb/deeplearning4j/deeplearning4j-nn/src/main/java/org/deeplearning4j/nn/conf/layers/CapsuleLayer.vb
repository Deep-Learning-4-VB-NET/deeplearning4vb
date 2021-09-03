Imports System
Imports System.Collections.Generic
Imports lombok
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InputTypeRecurrent = org.deeplearning4j.nn.conf.inputs.InputType.InputTypeRecurrent
Imports Type = org.deeplearning4j.nn.conf.inputs.InputType.Type
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports SameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports CapsuleUtils = org.deeplearning4j.util.CapsuleUtils
Imports ValidationUtils = org.deeplearning4j.util.ValidationUtils
Imports SDIndex = org.nd4j.autodiff.samediff.SDIndex
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @EqualsAndHashCode(callSuper = true) public class CapsuleLayer extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
	<Serializable>
	Public Class CapsuleLayer
		Inherits SameDiffLayer

		Private Const WEIGHT_PARAM As String = "weight"
		Private Const BIAS_PARAM As String = "bias"

		Private hasBias As Boolean = False
		Private inputCapsules As Long = 0
		Private inputCapsuleDimensions As Long = 0
		Private capsules As Integer
		Private capsuleDimensions As Integer
		Private routings As Integer = 3

		Public Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.hasBias = builder.hasBias_Conflict
			Me.inputCapsules = builder.inputCapsules_Conflict
			Me.inputCapsuleDimensions = builder.inputCapsuleDimensions_Conflict
			Me.capsules = builder.capsules_Conflict
			Me.capsuleDimensions = builder.capsuleDimensions_Conflict
			Me.routings = builder.routings_Conflict

			If capsules <= 0 OrElse capsuleDimensions <= 0 OrElse routings <= 0 Then
				Throw New System.ArgumentException("Invalid configuration for Capsule Layer (layer name = """ & layerName & """):" & " capsules, capsuleDimensions, and routings must be > 0.  Got: " & capsules & ", " & capsuleDimensions & ", " & routings)
			End If

			If inputCapsules < 0 OrElse inputCapsuleDimensions < 0 Then
				Throw New System.ArgumentException("Invalid configuration for Capsule Layer (layer name = """ & layerName & """):" & " inputCapsules and inputCapsuleDimensions must be >= 0 if set.  Got: " & inputCapsules & ", " & inputCapsuleDimensions)
			End If

		End Sub

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input for Capsule layer (layer name = """ & layerName & """): expect RNN input.  Got: " & inputType)
			End If

			If inputCapsules <= 0 OrElse inputCapsuleDimensions <= 0 Then
				Dim ir As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
				inputCapsules = ir.getSize()
				inputCapsuleDimensions = ir.getTimeSeriesLength()
			End If

		End Sub

		Public Overrides Function defineLayer(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable

			' input: [mb, inputCapsules, inputCapsuleDimensions]

			' [mb, inputCapsules, 1, inputCapsuleDimensions, 1]
			Dim expanded As SDVariable = sd.expandDims(sd.expandDims(input, 2), 4)

			' [mb, inputCapsules, capsules  * capsuleDimensions, inputCapsuleDimensions, 1]
			Dim tiled As SDVariable = sd.tile(expanded, 1, 1, capsules * capsuleDimensions, 1, 1)

			' [1, inputCapsules, capsules * capsuleDimensions, inputCapsuleDimensions]
			Dim weights As SDVariable = paramTable(WEIGHT_PARAM)

			' uHat is the matrix of prediction vectors between two capsules
			' [mb, inputCapsules, capsules, capsuleDimensions, 1]
			Dim uHat As SDVariable = weights.times(tiled).sum(True, 3).reshape(-1, inputCapsules, capsules, capsuleDimensions, 1)

			' b is the logits of the routing procedure
			' [mb, inputCapsules, capsules, 1, 1]
			Dim b As SDVariable = sd.zerosLike(uHat).get(SDIndex.all(), SDIndex.all(), SDIndex.all(), SDIndex.interval(0, 1), SDIndex.interval(0, 1))

			For i As Integer = 0 To routings - 1

				' c is the coupling coefficient, i.e. the edge weight between the 2 capsules
				' [mb, inputCapsules, capsules, 1, 1]
				Dim c As SDVariable = sd.nn_Conflict.softmax(b, 2)

				' [mb, 1, capsules, capsuleDimensions, 1]
				Dim s As SDVariable = c.times(uHat).sum(True, 1)
				If hasBias Then
					s = s.plus(paramTable(BIAS_PARAM))
				End If

				' v is the per capsule activations.  On the last routing iteration, this is output
				' [mb, 1, capsules, capsuleDimensions, 1]
				Dim v As SDVariable = CapsuleUtils.squash(sd, s, 3)

				If i = routings - 1 Then
					Return sd.squeeze(sd.squeeze(v, 1), 3)
				End If

				' [mb, inputCapsules, capsules, capsuleDimensions, 1]
				Dim vTiled As SDVariable = sd.tile(v, 1, CInt(inputCapsules), 1, 1, 1)

				' [mb, inputCapsules, capsules, 1, 1]
				b = b.plus(uHat.times(vTiled).sum(True, 3))
			Next i

			Return Nothing ' will always return in the loop
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			params.clear()
			params.addWeightParam(WEIGHT_PARAM, 1, inputCapsules, capsules * capsuleDimensions, inputCapsuleDimensions, 1)

			If hasBias Then
				params.addBiasParam(BIAS_PARAM, 1, 1, capsules, capsuleDimensions, 1)
			End If
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				For Each e As KeyValuePair(Of String, INDArray) In params.SetOfKeyValuePairs()
					If BIAS_PARAM.Equals(e.Key) Then
						e.Value.assign(0)
					ElseIf WEIGHT_PARAM.Equals(e.Key) Then
						WeightInitUtil.initWeights(inputCapsules * inputCapsuleDimensions, capsules * capsuleDimensions, New Long(){1, inputCapsules, capsules * capsuleDimensions, inputCapsuleDimensions, 1}, Me.weightInit, Nothing, "c"c, e.Value)
					End If
				Next e
			End Using
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Return InputType.recurrent(capsules, capsuleDimensions)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer.Builder<Builder>
		Public Class Builder
			Inherits SameDiffLayer.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field capsules was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend capsules_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field capsuleDimensions was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend capsuleDimensions_Conflict As Integer

'JAVA TO VB CONVERTER NOTE: The field routings was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend routings_Conflict As Integer = 3

'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasBias_Conflict As Boolean = False

'JAVA TO VB CONVERTER NOTE: The field inputCapsules was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend inputCapsules_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field inputCapsuleDimensions was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend inputCapsuleDimensions_Conflict As Integer = 0

			Public Sub New(ByVal capsules As Integer, ByVal capsuleDimensions As Integer)
				Me.New(capsules, capsuleDimensions, 3)
			End Sub

			Public Sub New(ByVal capsules As Integer, ByVal capsuleDimensions As Integer, ByVal routings As Integer)
				MyBase.New()
				Me.setCapsules(capsules)
				Me.setCapsuleDimensions(capsuleDimensions)
				Me.setRoutings(routings)
			End Sub

			Public Overrides Function build(Of E As Layer)() As E
				Return CType(New CapsuleLayer(Me), E)
			End Function

			''' <summary>
			''' Set the number of capsules to use. </summary>
			''' <param name="capsules">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter capsules was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function capsules(ByVal capsules_Conflict As Integer) As Builder
				Me.setCapsules(capsules_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Set the number dimensions of each capsule </summary>
			''' <param name="capsuleDimensions">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter capsuleDimensions was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function capsuleDimensions(ByVal capsuleDimensions_Conflict As Integer) As Builder
				Me.setCapsuleDimensions(capsuleDimensions_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Set the number of dynamic routing iterations to use.
			''' The default is 3 (recommendedded in Dynamic Routing Between Capsules) </summary>
			''' <param name="routings">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter routings was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function routings(ByVal routings_Conflict As Integer) As Builder
				Me.setRoutings(routings_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Usually inferred automatically. </summary>
			''' <param name="inputCapsules">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter inputCapsules was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function inputCapsules(ByVal inputCapsules_Conflict As Integer) As Builder
				Me.setInputCapsules(inputCapsules_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Usually inferred automatically. </summary>
			''' <param name="inputCapsuleDimensions">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter inputCapsuleDimensions was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function inputCapsuleDimensions(ByVal inputCapsuleDimensions_Conflict As Integer) As Builder
				Me.setInputCapsuleDimensions(inputCapsuleDimensions_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Usually inferred automatically. </summary>
			''' <param name="inputShape">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter inputShape was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function inputShape(ParamArray ByVal inputShape_Conflict() As Integer) As Builder
				Dim input() As Integer = ValidationUtils.validate2NonNegative(inputShape_Conflict, False, "inputShape")
				Me.setInputCapsules(input(0))
				Me.setInputCapsuleDimensions(input(1))
				Return Me
			End Function

			''' <summary>
			''' Sets whether to use bias.  False by default. </summary>
			''' <param name="hasBias">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter hasBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasBias(ByVal hasBias_Conflict As Boolean) As Builder
				Me.setHasBias(hasBias_Conflict)
				Return Me
			End Function

		End Class
	End Class

End Namespace