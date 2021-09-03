Imports System
Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation.classification
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastTo = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastTo
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports JsonMappers = org.nd4j.serde.json.JsonMappers
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports InvalidTypeIdException = org.nd4j.shade.jackson.databind.exc.InvalidTypeIdException

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

Namespace org.nd4j.evaluation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public abstract class BaseEvaluation<T extends BaseEvaluation> implements IEvaluation<T>
	Public MustInherit Class BaseEvaluation(Of T As BaseEvaluation)
		Implements IEvaluation(Of T)

		Public MustOverride Function newInstance() As IEvaluation
		Public MustOverride Function getValue(ByVal metric As IMetric) As Double Implements IEvaluation(Of T).getValue
		Public MustOverride Function stats() As String Implements IEvaluation(Of T).stats
		Public MustOverride Sub reset() Implements IEvaluation(Of T).reset
		Public MustOverride Sub merge(ByVal other As T) Implements IEvaluation(Of T).merge
		Public MustOverride Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal networkPredictions As INDArray, ByVal maskArray As INDArray, ByVal recordMetaData As IList(Of T1))

		''' <param name="yaml">  YAML representation </param>
		''' <param name="clazz"> Class </param>
		''' @param <T>   Type to return </param>
		''' <returns> Evaluation instance </returns>
		Public Shared Function fromYaml(Of T As IEvaluation)(ByVal yaml As String, ByVal clazz As Type(Of T)) As T
			Try
				Return JsonMappers.YamlMapper.readValue(yaml, clazz)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <param name="json">  Jason representation of the evaluation instance </param>
		''' <param name="clazz"> Class </param>
		''' @param <T>   Type to return </param>
		''' <returns> Evaluation instance </returns>
		Public Shared Function fromJson(Of T As IEvaluation)(ByVal json As String, ByVal clazz As Type(Of T)) As T
			Try
				Return JsonMappers.Mapper.readValue(json, clazz)
			Catch e As InvalidTypeIdException
				If e.Message.contains("Could not resolve type id") Then
					Try
						Return CType(attempFromLegacyFromJson(json, e), T)
'JAVA TO VB CONVERTER NOTE: The variable t was renamed since Visual Basic does not allow local variables with the same name as method-level generic type parameters:
					Catch t_Conflict As Exception
						Throw New Exception("Cannot deserialize from JSON - JSON is invalid?", t_Conflict)
					End Try
				End If
				Throw New Exception(e)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Attempt to load DL4J IEvaluation JSON from 1.0.0-beta2 or earlier.
		''' Given IEvaluation classes were moved to ND4J with no major changes, a simple "find and replace" for the class
		''' names is used.
		''' </summary>
		''' <param name="json">              JSON to attempt to deserialize </param>
		''' <param name="originalException"> Original exception to be re-thrown if it isn't legacy JSON </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected static <T extends IEvaluation> T attempFromLegacyFromJson(String json, org.nd4j.shade.jackson.databind.exc.InvalidTypeIdException originalException) throws org.nd4j.shade.jackson.databind.exc.InvalidTypeIdException
		Protected Friend Shared Function attempFromLegacyFromJson(Of T As IEvaluation)(ByVal json As String, ByVal originalException As InvalidTypeIdException) As T
			If json.Contains("org.deeplearning4j.eval.Evaluation") Then
				Dim newJson As String = json.replaceAll("org.deeplearning4j.eval.Evaluation", "org.nd4j.evaluation.classification.Evaluation")
				Return CType(fromJson(newJson, GetType(Evaluation)), T)
			End If

			If json.Contains("org.deeplearning4j.eval.EvaluationBinary") Then
				Dim newJson As String = json.replaceAll("org.deeplearning4j.eval.EvaluationBinary", "org.nd4j.evaluation.classification.EvaluationBinary").replaceAll("org.deeplearning4j.eval.ROC", "org.nd4j.evaluation.classification.ROC").replaceAll("org.deeplearning4j.eval.curves.", "org.nd4j.evaluation.curves.")
				Return CType(fromJson(newJson, GetType(EvaluationBinary)), T)
			End If

			If json.Contains("org.deeplearning4j.eval.EvaluationCalibration") Then
				Dim newJson As String = json.replaceAll("org.deeplearning4j.eval.EvaluationCalibration", "org.nd4j.evaluation.classification.EvaluationCalibration").replaceAll("org.deeplearning4j.eval.curves.", "org.nd4j.evaluation.curves.")
				Return CType(fromJson(newJson, GetType(EvaluationCalibration)), T)
			End If

			If json.Contains("org.deeplearning4j.eval.ROCBinary") Then
				Dim newJson As String = json.replaceAll("org.deeplearning4j.eval.ROCBinary", "org.nd4j.evaluation.classification.ROCBinary").replaceAll("org.deeplearning4j.eval.ROC", "org.nd4j.evaluation.classification.ROC").replaceAll("org.deeplearning4j.eval.curves.", "org.nd4j.evaluation.curves.")

				Return CType(fromJson(newJson, GetType(ROCBinary)), T)
			End If

			If json.Contains("org.deeplearning4j.eval.ROCMultiClass") Then
				Dim newJson As String = json.replaceAll("org.deeplearning4j.eval.ROCMultiClass", "org.nd4j.evaluation.classification.ROCMultiClass").replaceAll("org.deeplearning4j.eval.ROC", "org.nd4j.evaluation.classification.ROC").replaceAll("org.deeplearning4j.eval.curves.", "org.nd4j.evaluation.curves.")
				Return CType(fromJson(newJson, GetType(ROCMultiClass)), T)
			End If

			If json.Contains("org.deeplearning4j.eval.ROC") Then 'Has to be checked after ROCBinary/ROCMultiClass due to it being a prefix
				Dim newJson As String = json.replaceAll("org.deeplearning4j.eval.ROC", "org.nd4j.evaluation.classification.ROC").replaceAll("org.deeplearning4j.eval.curves.", "org.nd4j.evaluation.curves.")
				Return CType(fromJson(newJson, GetType(ROC)), T)
			End If

			If json.Contains("org.deeplearning4j.eval.RegressionEvaluation") Then
				Dim newJson As String = json.replaceAll("org.deeplearning4j.eval.RegressionEvaluation", "org.nd4j.evaluation.regression.RegressionEvaluation")
				Return CType(fromJson(newJson, GetType(RegressionEvaluation)), T)
			End If

			Throw originalException
		End Function

		Public Shared Function reshapeAndExtractNotMasked(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal mask As INDArray, ByVal axis As Integer) As Triple(Of INDArray, INDArray, INDArray)

			If labels.rank() = 2 Then
				Preconditions.checkState(axis = 1, "Only axis=1 is supported 2d data - got axis=%s for labels array shape %ndShape", axis, labels)
				If mask Is Nothing Then
					'no-op
					Return New Triple(Of INDArray, INDArray, INDArray)(labels, predictions, Nothing)
				Else
					'2 possible cases: per-output masking, and per example masking
					If mask.rank() = 1 OrElse mask.ColumnVector Then
						Dim notMaskedCount As Integer = mask.neq(0.0).castTo(DataType.INT).sumNumber().intValue()
						If notMaskedCount = 0 Then
							'All steps masked - nothing left to evaluate
							Return Nothing
						End If
						If notMaskedCount = mask.length() Then
							'No masked steps - returned as-is
							Return New Triple(Of INDArray, INDArray, INDArray)(labels, predictions, Nothing)
						End If
						Dim arr() As Integer = mask.toIntVector()
						Dim idxs(notMaskedCount - 1) As Integer
						Dim pos As Integer = 0
						For i As Integer = 0 To arr.Length - 1
							If arr(i) <> 0 Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idxs[pos++] = i;
								idxs(pos) = i
									pos += 1
							End If
						Next i
						Dim retLabel As INDArray = Nd4j.pullRows(labels, 1, idxs, "c"c)
						Dim retPredictions As INDArray = Nd4j.pullRows(predictions, 1, idxs, "c"c)
						Return New Triple(Of INDArray, INDArray, INDArray)(retLabel, retPredictions, Nothing)
					Else
						Preconditions.checkState(labels.equalShapes(mask), "If a mask array is present for 2d data, it must either be a vector (column vector)" & " or have shape equal to the labels (for per-output masking, when supported). Got labels shape %ndShape, mask shape %ndShape", labels, mask)
						'Assume evaluation instances with per-output masking will handle that themselves (or throw exception if not supported)
						Return New Triple(Of INDArray, INDArray, INDArray)(labels, predictions, mask)
					End If
				End If
			ElseIf labels.rank() = 3 OrElse labels.rank() = 4 OrElse labels.rank() = 5 Then
				If mask Is Nothing Then
					Return reshapeSameShapeTo2d(axis, labels, predictions, mask)
				Else
					If labels.rank() = 3 Then
						If mask.rank() = 2 Then
							'Per time step masking
							Dim p As Pair(Of INDArray, INDArray) = EvaluationUtils.extractNonMaskedTimeSteps(labels, predictions, mask)
							If p Is Nothing Then
								Return Nothing
							End If
							Return New Triple(Of INDArray, INDArray, INDArray)(p.First, p.Second, Nothing)
						Else
							'Per output mask
							Preconditions.checkState(labels.equalShapes(mask), "If a mask array is present for 3d data, it must either be 2d (shape [minibatch, sequenceLength])" & " or have shape equal to the labels (for per-output masking, when supported). Got labels shape %ndShape, mask shape %ndShape", labels, mask)
							'Assume evaluation instances with per-output masking will handle that themselves (or throw exception if not supported)
							'Just reshape to 2d

							Return reshapeSameShapeTo2d(axis, labels, predictions, mask)
						End If
					Else
						If labels.equalShapes(mask) Then
							'Per output masking case
							Return reshapeSameShapeTo2d(axis, labels, predictions, mask)
						ElseIf mask.rank() = 1 Then
							'Treat 1D mask as per-example masking
							Preconditions.checkState(mask.length() = labels.size(0), "For rank 4 labels with shape %ndShape and 1d" & " mask of shape %ndShape, the mask array length must equal labels dimension 0 size", labels, mask)
							Dim reshape() As Long = ArrayUtil.nTimes(labels.rank(), 1L)
							reshape(0) = mask.size(0)
							Dim mReshape As INDArray = mask.reshape(reshape)
							Dim bMask As INDArray = Nd4j.createUninitialized(mask.dataType(), labels.shape())
							Dim b As New BroadcastTo(mReshape, labels.shape(), bMask)
							Nd4j.exec(b)
							Return reshapeSameShapeTo2d(axis, labels, predictions, bMask)
						ElseIf mask.rank() = labels.rank() AndAlso Shape.areShapesBroadcastable(mask.shape(), labels.shape()) Then
							'Same rank, but different shape -> broadcast
							Dim bMask As INDArray = Nd4j.createUninitialized(mask.dataType(), labels.shape())
							Dim b As New BroadcastTo(mask, labels.shape(), bMask)
							Nd4j.exec(b)
							Return reshapeSameShapeTo2d(axis, labels, predictions, bMask)
						End If
						Throw New System.NotSupportedException("Evaluation case not supported: labels shape " & Arrays.toString(labels.shape()) & " with mask shape " & Arrays.toString(mask.shape()))
					End If
				End If
			Else
				Throw New System.InvalidOperationException("Unknown array type passed to evaluation: labels array rank " & labels.rank() & " with shape " & labels.shapeInfoToString() & ". Labels and predictions must always be rank 2 or higher, with leading dimension being minibatch dimension")
			End If
		End Function

		Private Shared Function reshapeSameShapeTo2d(ByVal axis As Integer, ByVal labels As INDArray, ByVal predictions As INDArray, ByVal mask As INDArray) As Triple(Of INDArray, INDArray, INDArray)
			Dim permuteDims(labels.rank() - 1) As Integer
			Dim j As Integer=0
			Dim i As Integer=0
			Do While i<labels.rank()
				If i = axis Then
					i += 1
					Continue Do
				End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: permuteDims[j++] = i;
				permuteDims(j) = i
					j += 1
				i += 1
			Loop
			permuteDims(j) = axis
			Dim size0 As Long = 1
			For i As Integer = 0 To permuteDims.Length - 2
				size0 *= labels.size(permuteDims(i))
			Next i

			Dim lOut As INDArray = labels.permute(permuteDims).dup("c"c).reshape("c"c,size0, labels.size(axis))
			Dim pOut As INDArray = predictions.permute(permuteDims).dup("c"c).reshape("c"c,size0, labels.size(axis))
			Dim mOut As INDArray = If(mask Is Nothing, Nothing, mask.permute(permuteDims).dup("c"c).reshape("c"c,size0, labels.size(axis)))

			Return New Triple(Of INDArray, INDArray, INDArray)(lOut, pOut, mOut)
		End Function

		Public Overridable Sub eval(ByVal labels As INDArray, ByVal networkPredictions As INDArray) Implements IEvaluation(Of T).eval
			eval(labels, networkPredictions, Nothing, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void eval(@NonNull INDArray labels, @NonNull final org.nd4j.linalg.api.ndarray.INDArray predictions, final java.util.List<? extends java.io.Serializable> recordMetaData)
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Public Overridable Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal recordMetaData As IList(Of T1))
			eval(labels, predictions, Nothing, recordMetaData)
		End Sub

		Public Overridable Sub eval(ByVal labels As INDArray, ByVal networkPredictions As INDArray, ByVal maskArray As INDArray) Implements IEvaluation(Of T).eval
			eval(labels, networkPredictions, maskArray, Nothing)
		End Sub

		Public Overridable Sub evalTimeSeries(ByVal labels As INDArray, ByVal predicted As INDArray) Implements IEvaluation(Of T).evalTimeSeries
			evalTimeSeries(labels, predicted, Nothing)
		End Sub

		Public Overridable Sub evalTimeSeries(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal labelsMask As INDArray) Implements IEvaluation(Of T).evalTimeSeries
			Dim pair As Pair(Of INDArray, INDArray) = EvaluationUtils.extractNonMaskedTimeSteps(labels, predictions, labelsMask)
			If pair Is Nothing Then
				'No non-masked steps
				Return
			End If
			Dim labels2d As INDArray = pair.First
			Dim predicted2d As INDArray = pair.Second

			eval(labels2d, predicted2d)
		End Sub

		''' <returns> JSON representation of the evaluation instance </returns>
		Public Overridable Function toJson() As String Implements IEvaluation(Of T).toJson
			Try
				Return JsonMappers.Mapper.writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		Public Overrides Function ToString() As String
			Return stats()
		End Function

		''' <returns> YAML  representation of the evaluation instance </returns>
		Public Overridable Function toYaml() As String Implements IEvaluation(Of T).toYaml
			Try
				Return JsonMappers.YamlMapper.writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace