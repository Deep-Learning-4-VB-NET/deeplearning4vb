Imports System
Imports System.Collections.Generic
Imports IntOpenHashSet = it.unimi.dsi.fastutil.ints.IntOpenHashSet
Imports IntSet = it.unimi.dsi.fastutil.ints.IntSet
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports CircularFifoQueue = org.apache.commons.collections4.queue.CircularFifoQueue
Imports org.deeplearning4j.rl4j.experience
Imports Random = org.nd4j.linalg.api.rng.Random

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

Namespace org.deeplearning4j.rl4j.learning.sync


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ExpReplay<A> implements IExpReplay<A>
	Public Class ExpReplay(Of A)
		Implements IExpReplay(Of A)

'JAVA TO VB CONVERTER NOTE: The field batchSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly batchSize_Conflict As Integer
		Private ReadOnly rnd As Random

		'Implementing this as a circular buffer queue
		Private storage As CircularFifoQueue(Of StateActionRewardState(Of A))

		Public Sub New(ByVal maxSize As Integer, ByVal batchSize As Integer, ByVal rnd As Random)
			Me.batchSize_Conflict = batchSize
			Me.rnd = rnd
			storage = New CircularFifoQueue(Of StateActionRewardState(Of A))(maxSize)
		End Sub

		Public Overridable Function getBatch(ByVal size As Integer) As List(Of StateActionRewardState(Of A))
			Dim batch As New List(Of StateActionRewardState(Of A))(size)
			Dim storageSize As Integer = storage.size()
			Dim actualBatchSize As Integer = Math.Min(storageSize, size)

			Dim actualIndex(actualBatchSize - 1) As Integer
			Dim set As IntSet = New IntOpenHashSet()
			For i As Integer = 0 To actualBatchSize - 1
				Dim [next] As Integer = rnd.nextInt(storageSize)
				Do While set.contains([next])
					[next] = rnd.nextInt(storageSize)
				Loop
				set.add([next])
				actualIndex(i) = [next]
			Next i

			For i As Integer = 0 To actualBatchSize - 1
				Dim trans As StateActionRewardState(Of A) = storage.get(actualIndex(i))
				batch.Add(trans.dup())
			Next i

			Return batch
		End Function

		Public Overridable Function getBatch() As List(Of StateActionRewardState(Of A)) Implements IExpReplay(Of A).getBatch
			Return getBatch(batchSize_Conflict)
		End Function

		Public Overridable Sub store(ByVal stateActionRewardState As StateActionRewardState(Of A)) Implements IExpReplay(Of A).store
			storage.add(stateActionRewardState)
			'log.info("size: "+storage.size());
		End Sub

		Public Overridable ReadOnly Property DesignatedBatchSize As Integer Implements IExpReplay(Of A).getDesignatedBatchSize
			Get
				Return batchSize_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property BatchSize As Integer Implements IExpReplay(Of A).getBatchSize
			Get
				Dim storageSize As Integer = storage.size()
				Return Math.Min(storageSize, batchSize_Conflict)
			End Get
		End Property

	End Class

End Namespace