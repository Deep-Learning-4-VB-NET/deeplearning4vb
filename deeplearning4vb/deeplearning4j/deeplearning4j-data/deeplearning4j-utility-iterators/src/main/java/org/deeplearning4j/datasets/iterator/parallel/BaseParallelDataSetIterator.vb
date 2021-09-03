Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports ParallelDataSetIterator = org.nd4j.linalg.dataset.api.iterator.ParallelDataSetIterator
Imports InequalityHandling = org.nd4j.linalg.dataset.api.iterator.enums.InequalityHandling
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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
'ORIGINAL LINE: @Slf4j public abstract class BaseParallelDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.ParallelDataSetIterator
	<Serializable>
	Public MustInherit Class BaseParallelDataSetIterator
		Implements ParallelDataSetIterator

		Protected Friend counter As New AtomicLong(0)

		Protected Friend inequalityHandling As InequalityHandling
		Protected Friend numProducers As Integer

		Protected Friend allDepleted As New AtomicBoolean(False)
		Protected Friend states As MultiBoolean
		Protected Friend resetTracker As MultiBoolean

		Protected Friend producerAffinity As New ThreadLocal(Of Integer)()


		Protected Friend Sub New(ByVal numProducers As Integer)
			states = New MultiBoolean(numProducers, True)
			resetTracker = New MultiBoolean(numProducers, False, True)
			Me.numProducers = numProducers
		End Sub


		Public Overridable Function hasNext() As Boolean
			' if all producers are depleted - there's nothing to do here then
			If states.allFalse() OrElse allDepleted.get() Then
				Return False
			End If

			Dim curIdx As Integer = CurrentProducerIndex

'JAVA TO VB CONVERTER NOTE: The local variable hasNext was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim hasNext_Conflict As Boolean = hasNextFor(curIdx)

			If hasNext_Conflict Then
				Return True
			Else
				states.set(hasNext_Conflict, curIdx)
			End If

			If states.allFalse() Then
				Return False
			End If

			Select Case inequalityHandling
				' FIXME: RESET should be applicable ONLY to producers which return TRUE for resetSupported();
				Case InequalityHandling.RESET
					resetTracker.set(True, curIdx)

					' we don't want to have endless loop here, so we only do reset until all producers depleted at least once
					If resetTracker.allTrue() Then
						allDepleted.set(True)
						Return False
					End If

					reset(curIdx)

					' triggering possible adsi underneath
					hasNextFor(curIdx)

					Return True
				Case InequalityHandling.RELOCATE
					' TODO: transparent switch to next producer should happen here
					Do While Not hasNext_Conflict
						stepForward()
						hasNext_Conflict = hasNextFor(CurrentProducerIndex)
						states.set(hasNext_Conflict, CurrentProducerIndex)

						If states.allFalse() Then
							Return False
						End If
					Loop

					Return True
				Case InequalityHandling.PASS_NULL
					' we just return true here, no matter what's up
					Return True
				Case InequalityHandling.STOP_EVERYONE
					If Not states.allTrue() Then
						Return False
					End If

					Return True
				Case Else
					Throw New ND4JIllegalStateException("Unknown InequalityHanding option was passed in: " & inequalityHandling)
			End Select
		End Function

		Public Overridable Function [next]() As DataSet
			Dim ds As DataSet = nextFor(CurrentProducerIndex)
			stepForward()
			Return ds
		End Function

		Protected Friend Overridable ReadOnly Property CurrentProducerIndex As Integer
			Get
				Return CInt(Math.Truncate(counter.get() Mod numProducers))
			End Get
		End Property

		Protected Friend Overridable Sub stepForward()
			counter.getAndIncrement()
		End Sub

		Public Overridable Sub reset()
			For i As Integer = 0 To numProducers - 1
				reset(i)
				states.set(True, i)
				resetTracker.set(False, i)
			Next i

			allDepleted.set(False)
		End Sub

		Public Overridable Sub attachThread(ByVal producer As Integer) Implements ParallelDataSetIterator.attachThread
			producerAffinity.set(producer)
		End Sub

		Public Overridable Function hasNextFor() As Boolean Implements ParallelDataSetIterator.hasNextFor
			If producerAffinity.get() Is Nothing Then
				Throw New ND4JIllegalStateException("attachThread(int) should be called prior to this call")
			End If

			Return hasNextFor(producerAffinity.get())
		End Function

		Public Overridable Function nextFor() As DataSet Implements ParallelDataSetIterator.nextFor
			If producerAffinity.get() Is Nothing Then
				Throw New ND4JIllegalStateException("attachThread(int) should be called prior to this call")
			End If

			Return nextFor(producerAffinity.get())
		End Function

		Public MustOverride Function hasNextFor(ByVal consumer As Integer) As Boolean Implements ParallelDataSetIterator.hasNextFor

		Public MustOverride Function nextFor(ByVal consumer As Integer) As DataSet Implements ParallelDataSetIterator.nextFor

		Protected Friend MustOverride Sub reset(ByVal consumer As Integer)

		Public Overridable Function totalOutcomes() As Integer
			Return 0
		End Function

		Public Overridable Function resetSupported() As Boolean
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean
			Return False
		End Function

		Public Overridable Function batch() As Integer
			Return 0
		End Function

		Public Overridable Function [next](ByVal num As Integer) As DataSet
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function inputColumns() As Integer
			Return 0
		End Function

		Public Overridable Property PreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Throw New System.NotSupportedException()
			End Set
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Sub remove()
			' no-op
		End Sub
	End Class

End Namespace