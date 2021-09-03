Imports System.Collections.Generic
Imports System.Linq
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports InferenceObservable = org.deeplearning4j.parallelism.inference.InferenceObservable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetUtil = org.nd4j.linalg.dataset.api.DataSetUtil
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.parallelism.inference.observers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BatchedInferenceObservable extends BasicInferenceObservable implements org.deeplearning4j.parallelism.inference.InferenceObservable
	Public Class BatchedInferenceObservable
		Inherits BasicInferenceObservable
		Implements InferenceObservable

		Private inputs As IList(Of INDArray()) = New List(Of INDArray())()
		Private inputMasks As IList(Of INDArray()) = New List(Of INDArray())()
		Private outputs As IList(Of INDArray()) = New List(Of INDArray())()
'JAVA TO VB CONVERTER NOTE: The field counter was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private counter_Conflict As New AtomicInteger(0)
'JAVA TO VB CONVERTER NOTE: The field position was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private position_Conflict As New ThreadLocal(Of Integer)()
		Private outputBatchInputArrays As IList(Of Integer()) = New List(Of Integer())()

		Private ReadOnly locker As New Object()

		Private realLocker As New ReentrantReadWriteLock()
		Private isLocked As New AtomicBoolean(False)
		Private isReadLocked As New AtomicBoolean(False)

		Public Sub New()

		End Sub

		Public Overrides Sub addInput(ByVal input() As INDArray, ByVal inputMasks() As INDArray) Implements InferenceObservable.addInput
			SyncLock locker
				inputs.Add(input)
				Me.inputMasks.Add(inputMasks)
				position_Conflict.set(counter_Conflict.getAndIncrement())

				If isReadLocked.get() Then
					realLocker.readLock().unlock()
				End If
			End SyncLock
		End Sub

		Public Overrides ReadOnly Property InputBatches As IList(Of Pair(Of INDArray(), INDArray())) Implements InferenceObservable.getInputBatches
			Get
				realLocker.writeLock().lock()
				isLocked.set(True)
    
				outputBatchInputArrays.Clear()
    
				' this method should pile individual examples into single batch
    
				If counter_Conflict.get() > 1 Then
    
					Dim pos As Integer = 0
					Dim [out] As IList(Of Pair(Of INDArray(), INDArray())) = New List(Of Pair(Of INDArray(), INDArray()))()
					Dim numArrays As Integer = inputs(0).Length
					Do While pos < inputs.Count
    
						'First: determine which we can actually batch...
						Dim lastPossible As Integer = pos
						For i As Integer = pos+1 To inputs.Count - 1
							If canBatch(inputs(pos), inputs(i)) Then
								lastPossible = i
							Else
								Exit For
							End If
						Next i
    
						Dim countToMerge As Integer = lastPossible-pos+1
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim featuresToMerge[][] As INDArray = new INDArray[countToMerge][0]
						Dim featuresToMerge()() As INDArray = RectangularArrays.RectangularINDArrayArray(countToMerge, 0)
						Dim fMasksToMerge()() As INDArray = Nothing
						Dim fPos As Integer = 0
						For i As Integer = pos To lastPossible
							featuresToMerge(fPos) = inputs(i)
    
							If inputMasks(i) IsNot Nothing Then
								If fMasksToMerge Is Nothing Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: fMasksToMerge = new INDArray[countToMerge][0]
									fMasksToMerge = RectangularArrays.RectangularINDArrayArray(countToMerge, 0)
									For j As Integer = 0 To countToMerge - 1
										fMasksToMerge(j) = Nothing
									Next j
								End If
								fMasksToMerge(fPos) = inputMasks(i)
							End If
							fPos += 1
						Next i
    
						Dim merged As Pair(Of INDArray(), INDArray()) = DataSetUtil.mergeFeatures(featuresToMerge, fMasksToMerge)
						[out].Add(merged)
    
						outputBatchInputArrays.Add(New Integer(){pos, lastPossible})
						pos = lastPossible+1
					Loop
					realLocker.writeLock().unlock()
					Return [out]
				Else
					outputBatchInputArrays.Add(New Integer(){0, 0})
					realLocker.writeLock().unlock()
					Return Collections.singletonList(New Pair(Of )(inputs(0), inputMasks(0)))
				End If
			End Get
		End Property

		Private Shared Function canBatch(ByVal first() As INDArray, ByVal candidate() As INDArray) As Boolean
			'Check if we can batch these inputs into the one array. This isn't always possible - for example, some fully
			' convolutional nets can support different input image sizes
			'For now: let's simply require that the inputs have the same shape
			'In the future: we'll intelligently handle the RNN variable length case
			'Note also we can ignore input masks here - they should have shared dimensions with the input, thus if the
			' inputs can be batched, so can the masks
			For i As Integer = 0 To first.Length - 1
				If Not first(i).shape().SequenceEqual(candidate(i).shape()) Then
					Return False
				End If
			Next i
			Return True
		End Function

		Public Overridable Overloads WriteOnly Property OutputBatches Implements InferenceObservable.setOutputBatches As IList(Of INDArray())
			Set(ByVal output As IList(Of INDArray()))
				'this method should split batched output INDArray[] into multiple separate INDArrays
				Dim countNumInputBatches As Integer = 0 'Counter for total number of input batches processed
				For outBatchNum As Integer = 0 To output.Count - 1 'Iterate over output batch
					Dim currBatchOutputs() As INDArray = output(outBatchNum)
					Dim inputBatchIdxs() As Integer = outputBatchInputArrays(outBatchNum)
					Dim inputBatchCount As Integer = inputBatchIdxs(1) - inputBatchIdxs(0) + 1
					For i As Integer = 0 To inputBatchCount - 1
						outputs.Add(New INDArray(currBatchOutputs.Length - 1){})
					Next i
    
					' pull back results for individual input batches
					Dim firstInputBatch As Integer = countNumInputBatches
					For outputNumber As Integer = 0 To currBatchOutputs.Length - 1 'Iterate over net outputs
						Dim split() As INDArray = splitExamples(currBatchOutputs(outputNumber), inputBatchIdxs(0), inputBatchIdxs(1))
    
						Dim currentInputBatch As Integer = firstInputBatch
						'Iterate over input batch (examples) - note that each output batch is made up of 1 or more input batches
						For inputInBatch As Integer = 0 To inputBatchCount - 1
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: outputs.get(currentInputBatch++)[outputNumber] = split[inputInBatch];
							outputs(currentInputBatch)(outputNumber) = split(inputInBatch)
								currentInputBatch += 1
    
							If outputNumber = 0 Then
								countNumInputBatches += 1
							End If
						Next inputInBatch
					Next outputNumber
				Next outBatchNum
    
				Me.setChanged()
				notifyObservers()
			End Set
		End Property

		Private Function splitExamples(ByVal netOutput As INDArray, ByVal firstInputComponent As Integer, ByVal lastInputComponent As Integer) As INDArray()

			Dim numSplits As Integer = lastInputComponent - firstInputComponent + 1
			If numSplits = 1 Then
				Return New INDArray(){netOutput}
			Else
				Dim [out](numSplits - 1) As INDArray
				Dim indices(netOutput.rank() - 1) As INDArrayIndex
				For i As Integer = 1 To indices.Length - 1
					indices(i) = NDArrayIndex.all()
				Next i
				Dim examplesSoFar As Integer = 0
				For inNum As Integer = 0 To numSplits - 1
					Dim inSizeEx As val = inputs(firstInputComponent + inNum)(0).size(0)
					indices(0) = NDArrayIndex.interval(examplesSoFar, examplesSoFar+inSizeEx)
					[out](inNum) = netOutput.get(indices)
					examplesSoFar += inSizeEx
				Next inNum
				Return [out]
			End If
		End Function

		''' <summary>
		''' PLEASE NOTE: This method is for tests only
		''' 
		''' @return
		''' </summary>
		Protected Friend Overridable ReadOnly Property Outputs As IList(Of INDArray())
			Get
				Return outputs
			End Get
		End Property

		Protected Friend Overridable Property Counter As Integer
			Set(ByVal value As Integer)
				counter_Conflict.set(value)
			End Set
			Get
				Return counter_Conflict.get()
			End Get
		End Property

		Public Overridable WriteOnly Property Position As Integer
			Set(ByVal pos As Integer)
				position_Conflict.set(pos)
			End Set
		End Property




		Public Overridable ReadOnly Property Locked As Boolean
			Get
				Dim lck As Boolean = Not realLocker.readLock().tryLock()
    
				Dim result As Boolean = lck OrElse isLocked.get()
    
				If Not result Then
					isReadLocked.set(True)
				End If
    
				Return result
			End Get
		End Property


		Public Overrides ReadOnly Property Output As INDArray() Implements InferenceObservable.getOutput
			Get
				' basically we should take care of splits here: each client should get its own part of output, wrt order number
				checkOutputException()
				Return outputs(position_Conflict.get())
			End Get
		End Property
	End Class

End Namespace