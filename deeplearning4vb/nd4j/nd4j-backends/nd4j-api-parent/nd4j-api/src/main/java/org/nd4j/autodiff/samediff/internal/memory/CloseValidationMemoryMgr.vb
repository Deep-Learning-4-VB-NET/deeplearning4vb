Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports org.nd4j.autodiff.samediff.internal
Imports org.nd4j.autodiff.samediff.internal
Imports InferenceSession = org.nd4j.autodiff.samediff.internal.InferenceSession
Imports SessionMemMgr = org.nd4j.autodiff.samediff.internal.SessionMemMgr
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
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

Namespace org.nd4j.autodiff.samediff.internal.memory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CloseValidationMemoryMgr extends AbstractMemoryMgr implements org.nd4j.autodiff.samediff.internal.SessionMemMgr
	Public Class CloseValidationMemoryMgr
		Inherits AbstractMemoryMgr
		Implements SessionMemMgr

		Private ReadOnly sd As SameDiff
		Private ReadOnly underlying As SessionMemMgr
		Private ReadOnly released As IDictionary(Of INDArray, Boolean) = New IdentityHashMap(Of INDArray, Boolean)()

		Public Sub New(ByVal sd As SameDiff, ByVal underlying As SessionMemMgr)
			Me.sd = sd
			Me.underlying = underlying
		End Sub

		Public Overrides Function allocate(ByVal detached As Boolean, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray Implements SessionMemMgr.allocate
			Dim [out] As INDArray = underlying.allocate(detached, dataType, shape)
			released([out]) = False
			Return [out]
		End Function

		Public Overrides Function allocate(ByVal detached As Boolean, ByVal descriptor As LongShapeDescriptor) As INDArray Implements SessionMemMgr.allocate
			Dim [out] As INDArray = underlying.allocate(detached, descriptor)
			released([out]) = False
			Return [out]
		End Function

		Public Overrides Sub release(ByVal array As INDArray) Implements SessionMemMgr.release
			Preconditions.checkState(released.ContainsKey(array), "Attempting to release an array that was not allocated by" & " this memory manager: id=%s", array.Id)
			If released(array) Then
				'Already released
				Dim [is] As InferenceSession = sd.getSessions().get(Thread.CurrentThread.getId())
				Dim arrayUseTracker As IdentityDependencyTracker(Of INDArray, InferenceSession.Dep) = [is].getArrayUseTracker()
				Dim dl As DependencyList(Of INDArray, InferenceSession.Dep) = arrayUseTracker.getDependencies(array)
				Console.WriteLine(dl)
				If dl.getDependencies() IsNot Nothing Then
					For Each d As InferenceSession.Dep In dl.getDependencies()
						Console.WriteLine(d & ": " & arrayUseTracker.isSatisfied(d))
					Next d
				End If
				If dl.getOrDependencies() IsNot Nothing Then
					For Each p As Pair(Of InferenceSession.Dep, InferenceSession.Dep) In dl.getOrDependencies()
						Console.WriteLine(p & " - (" & arrayUseTracker.isSatisfied(p.First) & "," & arrayUseTracker.isSatisfied(p.Second))
					Next p
				End If
			End If
			Preconditions.checkState(Not released(array), "Attempting to release an array that was already deallocated by" & " an earlier release call to this memory manager: id=%s", array.Id)
			log.trace("Released array: id = {}", array.Id)
			released(array) = True
		End Sub

		Public Overrides Sub Dispose() Implements SessionMemMgr.close
			underlying.Dispose()
		End Sub

		''' <summary>
		''' Check that all arrays have been released (after an inference call) except for the specified arrays.
		''' </summary>
		''' <param name="except"> Arrays that should not have been closed (usually network outputs) </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void assertAllReleasedExcept(@NonNull Collection<org.nd4j.linalg.api.ndarray.INDArray> except)
		Public Overridable Sub assertAllReleasedExcept(ByVal except As ICollection(Of INDArray))
			Dim allVarPhConst As ISet(Of INDArray) = Nothing

			For Each arr As INDArray In except
				If Not released.ContainsKey(arr) Then
					'Check if constant, variable or placeholder - maybe user requested that out
					If allVarPhConst Is Nothing Then
						allVarPhConst = identitySetAllConstPhVar()
					End If
					If allVarPhConst.Contains(arr) Then
						Continue For 'OK - output is a constant, variable or placeholder, hence it's fine it's not allocated by the memory manager
					End If

					Throw New System.InvalidOperationException("Array " & arr.Id & " was not originally allocated by the memory manager")
				End If

				Dim released As Boolean = Me.released(arr)
				If released Then
					Throw New System.InvalidOperationException("Specified output array (id=" & arr.Id & ") should not have been deallocated but was")
				End If
			Next arr

			Dim exceptSet As ISet(Of INDArray) = Collections.newSetFromMap(New IdentityHashMap(Of INDArray, Boolean)())
			exceptSet.addAll(except)

			Dim numNotClosed As Integer = 0
			Dim notReleased As ISet(Of INDArray) = Collections.newSetFromMap(New IdentityHashMap(Of INDArray, Boolean)())
			Dim [is] As InferenceSession = sd.getSessions().get(Thread.CurrentThread.getId())
			Dim arrayUseTracker As IdentityDependencyTracker(Of INDArray, InferenceSession.Dep) = [is].getArrayUseTracker()
			For Each e As KeyValuePair(Of INDArray, Boolean) In released.SetOfKeyValuePairs()
				Dim a As INDArray = e.Key
				If Not exceptSet.Contains(a) Then
					Dim b As Boolean = e.Value
					If Not b Then
						notReleased.Add(a)
						numNotClosed += 1
						log.info("Not released: array id {}", a.Id)
						Dim list As DependencyList(Of INDArray, InferenceSession.Dep) = arrayUseTracker.getDependencies(a)
						Dim l As IList(Of InferenceSession.Dep) = list.getDependencies()
						Dim l2 As IList(Of Pair(Of InferenceSession.Dep, InferenceSession.Dep)) = list.getOrDependencies()
						If l IsNot Nothing Then
							For Each d As InferenceSession.Dep In l
								If Not arrayUseTracker.isSatisfied(d) Then
									log.info("  Not satisfied: {}", d)
								End If
							Next d
						End If
						If l2 IsNot Nothing Then
							For Each d As Pair(Of InferenceSession.Dep, InferenceSession.Dep) In l2
								If Not arrayUseTracker.isSatisfied(d.First) AndAlso Not arrayUseTracker.isSatisfied(d.Second) Then
									log.info("   Not satisfied: {}", d)
								End If
							Next d
						End If
					End If
				End If
			Next e

			If numNotClosed > 0 Then
				Console.WriteLine(sd.summary())
				Throw New System.InvalidOperationException(numNotClosed & " arrays were not released but should have been")
			End If
		End Sub

		Protected Friend Overridable Function identitySetAllConstPhVar() As ISet(Of INDArray)
			Dim set As ISet(Of INDArray) = Collections.newSetFromMap(New IdentityHashMap(Of INDArray, Boolean)())
			For Each v As SDVariable In sd.variables()
				If v.getVariableType() = VariableType.VARIABLE OrElse v.getVariableType() = VariableType.CONSTANT OrElse v.getVariableType() = VariableType.PLACEHOLDER Then
					set.Add(v.Arr)
				End If
			Next v
			Return set
		End Function
	End Class

End Namespace