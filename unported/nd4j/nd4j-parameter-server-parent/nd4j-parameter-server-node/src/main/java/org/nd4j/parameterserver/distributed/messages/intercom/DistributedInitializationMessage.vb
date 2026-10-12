Imports System
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ExecutionMode = org.nd4j.parameterserver.distributed.enums.ExecutionMode
Imports WordVectorStorage = org.nd4j.parameterserver.distributed.logic.storage.WordVectorStorage
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports DistributedMessage = org.nd4j.parameterserver.distributed.messages.DistributedMessage
Imports InitializationAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.InitializationAggregation

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

Namespace org.nd4j.parameterserver.distributed.messages.intercom

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Builder @Data @Slf4j @Deprecated public class DistributedInitializationMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.DistributedMessage
	<Obsolete, Serializable>
	Public Class DistributedInitializationMessage
		Inherits BaseVoidMessage
		Implements DistributedMessage

		Protected Friend vectorLength As Integer
		Protected Friend numWords As Integer
		Protected Friend seed As Long
		Protected Friend useHs As Boolean
		Protected Friend useNeg As Boolean
		Protected Friend columnsPerShard As Integer

		Public Sub New(ByVal vectorLength As Integer, ByVal numWords As Integer, ByVal seed As Long, ByVal useHs As Boolean, ByVal useNeg As Boolean, ByVal columnsPerShard As Integer)
			MyBase.New(4)
			Me.vectorLength = vectorLength
			Me.numWords = numWords
			Me.seed = seed
			Me.useHs = useHs
			Me.useNeg = useNeg
			Me.columnsPerShard = columnsPerShard
		End Sub

		''' <summary>
		''' This method initializes shard storage with given data
		''' </summary>
		Public Overrides Sub processMessage()
			' protection check, we definitely don't want double spending here
			Dim syn0 As INDArray = storage.getArray(WordVectorStorage.SYN_0)
			Dim syn1 As INDArray = storage.getArray(WordVectorStorage.SYN_1)
			Dim syn1Neg As INDArray = storage.getArray(WordVectorStorage.SYN_1_NEGATIVE)
			Dim expTable As INDArray = storage.getArray(WordVectorStorage.EXP_TABLE)
			If syn0 Is Nothing Then
				log.info("sI_{} is starting initialization...", transport.ShardIndex)

				' we initialize only syn0/syn1/syn1neg and expTable
				' negTable will be initalized at driver level and will be shared via message
				Nd4j.Random.Seed = seed * (shardIndex + 1)

				If voidConfiguration.getExecutionMode() = ExecutionMode.AVERAGING Then
					' each shard has full own copy
					columnsPerShard = vectorLength
				ElseIf voidConfiguration.getExecutionMode() = ExecutionMode.SHARDED Then
					' each shard will have only part of the data
					If voidConfiguration.getNumberOfShards() - 1 = shardIndex Then
						Dim modulo As Integer = vectorLength Mod voidConfiguration.getNumberOfShards()
						If modulo <> 0 Then
							columnsPerShard += modulo
							log.info("Got inequal split. using higher number of elements: {}", columnsPerShard)
						End If
					End If
				End If

				Dim shardShape() As Integer = {numWords, columnsPerShard}

				syn0 = Nd4j.rand(shardShape, "c"c).subi(0.5).divi(vectorLength)

				If useHs Then
					syn1 = Nd4j.create(shardShape, "c"c)
				End If

				If useNeg Then
					syn1Neg = Nd4j.create(shardShape, "c"c)
				End If

				' we handle full exp table here
				expTable = initExpTable(100000)


				storage.setArray(WordVectorStorage.SYN_0, syn0)

				If useHs Then
					storage.setArray(WordVectorStorage.SYN_1, syn1)
				End If
				If useNeg Then
					storage.setArray(WordVectorStorage.SYN_1_NEGATIVE, syn1Neg)
				End If

				storage.setArray(WordVectorStorage.EXP_TABLE, expTable)

				Dim ia As New InitializationAggregation(CShort(Math.Truncate(voidConfiguration.getNumberOfShards())), transport.ShardIndex)
				ia.OriginatorId = Me.originatorId
				transport.sendMessage(ia)
			End If
		End Sub

		Protected Friend Overridable Function initExpTable(ByVal tableWidth As Integer) As INDArray
			Dim expTable(tableWidth - 1) As Double
			For i As Integer = 0 To expTable.Length - 1
				Dim tmp As Double = FastMath.exp((i / CDbl(expTable.Length) * 2 - 1) * 6)
				expTable(i) = tmp / (tmp + 1.0)
			Next i

			Return Nd4j.create(expTable)
		End Function
	End Class

End Namespace