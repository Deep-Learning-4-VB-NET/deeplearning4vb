Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports AsyncDataSetIterator = org.nd4j.linalg.dataset.AsyncDataSetIterator
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports InequalityHandling = org.nd4j.linalg.dataset.api.iterator.enums.InequalityHandling
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
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

Namespace org.deeplearning4j.datasets.iterator.parallel



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class JointParallelDataSetIterator extends BaseParallelDataSetIterator
	<Serializable>
	Public Class JointParallelDataSetIterator
		Inherits BaseParallelDataSetIterator

		Protected Friend asyncIterators As IList(Of DataSetIterator) = New List(Of DataSetIterator)()
		Protected Friend enforceSingleDevice As Boolean
		Protected Friend bufferSizePerDevice As Integer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public JointParallelDataSetIterator(@NonNull List<org.nd4j.linalg.dataset.api.iterator.DataSetIterator> iterators, boolean singleDeviceMode, int bufferSize, @NonNull InequalityHandling inequalityHandling)
		Public Sub New(ByVal iterators As IList(Of DataSetIterator), ByVal singleDeviceMode As Boolean, ByVal bufferSize As Integer, ByVal inequalityHandling As InequalityHandling)
			MyBase.New(iterators.size())
			Me.enforceSingleDevice = singleDeviceMode
			Me.bufferSizePerDevice = bufferSize
			Me.numProducers = iterators.size()
			Me.inequalityHandling = inequalityHandling

			If numProducers = 0 Then
				Throw New System.ArgumentException("You can't start ParallelDataSetIterator without input data")
			End If

			initializeIterators(iterators)
		End Sub

		Protected Friend Overridable Sub initializeIterators(ByVal originals As IList(Of DataSetIterator))
			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices

			Dim currentDevice As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()

			If originals.Count Mod numDevices <> 0 Then
				log.error("WARNING: number of splits doesn't match number of devices!")
			End If

			Dim cnt As Integer = 0
			For Each iterator As DataSetIterator In originals
				Dim cDev As Integer = cnt Mod numDevices
				asyncIterators.Add(New AsyncDataSetIterator(iterator, bufferSizePerDevice, True, cDev))
				cnt += 1
			Next iterator
		End Sub

		Public Overrides Function hasNextFor(ByVal consumer As Integer) As Boolean
			If consumer >= numProducers OrElse consumer < 0 Then
				Throw New ND4JIllegalStateException("Non-existent consumer was requested")
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return asyncIterators(consumer).hasNext()
		End Function


		Public Overrides Function nextFor(ByVal consumer As Integer) As DataSet
			If consumer >= numProducers OrElse consumer < 0 Then
				Throw New ND4JIllegalStateException("Non-existent consumer was requested")
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return asyncIterators(consumer).next()
		End Function

		Protected Friend Overrides Sub reset(ByVal consumer As Integer)
			If consumer >= numProducers OrElse consumer < 0 Then
				Throw New ND4JIllegalStateException("Non-existent consumer was requested")
			End If

			asyncIterators(consumer).reset()
		End Sub


		Public Class Builder
			Friend iterators As IList(Of DataSetIterator) = New List(Of DataSetIterator)()
'JAVA TO VB CONVERTER NOTE: The field enforceSingleDevice was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend enforceSingleDevice_Conflict As Boolean = True
			Friend bufferSize As Integer = 4
			Friend inequalityHandling As InequalityHandling

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull InequalityHandling inequalityHandling)
			Public Sub New(ByVal inequalityHandling As InequalityHandling)
				Me.inequalityHandling = inequalityHandling
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull List<org.nd4j.linalg.dataset.api.iterator.DataSetIterator> iterators, @NonNull InequalityHandling inequalityHandling)
			Public Sub New(ByVal iterators As IList(Of DataSetIterator), ByVal inequalityHandling As InequalityHandling)
				Me.inequalityHandling = inequalityHandling

				For Each iterator As DataSetIterator In iterators
					addSourceIterator(iterator)
				Next iterator
			End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addSourceIterator(@NonNull DataSetIterator iterator)
			Public Overridable Function addSourceIterator(ByVal iterator As DataSetIterator) As Builder
				If Not iterator.asyncSupported() Then
					Throw New System.ArgumentException("Source iterators should support async mode")
				End If

				'TODO: add strict equality check here, we don't want it equal
				If Not hasIterator(iterator) Then
					iterators.Add(iterator)
				Else
					Throw New System.ArgumentException("You can't put equal iterators into this joint iterator")
				End If

				Return Me
			End Function

			Protected Friend Overridable Function hasIterator(ByVal iterator As DataSetIterator) As Boolean
				For Each iter As DataSetIterator In iterators
					If iter Is iterator Then
						Return True
					End If
				Next iter

				Return False
			End Function

			Public Overridable Function setBufferSizePerSplit(ByVal bufferSize As Integer) As Builder
				Me.bufferSize = bufferSize
				Return Me
			End Function


			Public Overridable Function enforceSingleDevice(ByVal reallyEnforce As Boolean) As Builder
				Me.enforceSingleDevice_Conflict = reallyEnforce
				Return Me
			End Function


			Public Overridable Function build() As JointParallelDataSetIterator
				Dim jpdsi As New JointParallelDataSetIterator(iterators, enforceSingleDevice_Conflict, bufferSize, inequalityHandling)

				Return jpdsi
			End Function
		End Class
	End Class

End Namespace