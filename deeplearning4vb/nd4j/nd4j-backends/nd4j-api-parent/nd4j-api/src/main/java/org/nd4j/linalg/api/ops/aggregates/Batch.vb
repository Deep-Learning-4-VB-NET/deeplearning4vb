Imports System.Collections.Generic
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg.api.ops.aggregates


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Batch<T extends Aggregate>
	Public Class Batch(Of T As Aggregate)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.nd4j.linalg.api.buffer.DataBuffer paramsSurface;
		Private paramsSurface As DataBuffer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private static final int batchLimit = 512;
		Private Const batchLimit As Integer = 512

		' all aggregates within this batch
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private java.util.List<T> aggregates;
		Private aggregates As IList(Of T)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private T sample;
		Private sample As T
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int numAggregates;
		Private numAggregates As Integer

		''' <summary>
		''' This constructor takes List of Aggregates, and builds Batch instance, usable with Nd4j executioner.
		''' </summary>
		''' <param name="aggregates"> </param>
		Public Sub New(ByVal aggregates As IList(Of T))
			'if (aggregates.size() > batchLimit)
			'    throw new RuntimeException("Number of aggregates is higher then " + batchLimit + " elements, multiple batches should be issued.");

			Me.aggregates = aggregates
			Me.numAggregates = aggregates.Count

			' we fetch single sample for possible future use. not sure if will be used though
			Me.sample = aggregates(0)
		End Sub

		''' <summary>
		''' This method returns opNum for batched aggregate
		''' @return
		''' </summary>
		Public Overridable Function opNum() As Integer
			Return sample.opNum()
		End Function

		''' <summary>
		''' This method tries to append aggregate to the current batch, if it has free room
		''' </summary>
		''' <param name="aggregate">
		''' @return </param>
		Public Overridable Function append(ByVal aggregate As T) As Boolean
			If Not Full Then
				aggregates.Add(aggregate)
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' This method checks, if number of batched aggregates equals to maximum possible value
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Full As Boolean
			Get
				Return batchLimit = numAggregates
			End Get
		End Property


		''' <summary>
		''' Helper method to create batch from list of aggregates, for cases when list of aggregates is higher then batchLimit
		''' </summary>
		''' <param name="list"> </param>
		''' @param <U>
		''' @return </param>
		Public Shared Function getBatches(Of U As Aggregate)(ByVal list As IList(Of U)) As IList(Of Batch(Of U))
			Return getBatches(list, batchLimit)
		End Function

		''' <summary>
		''' Helper method to create batch from list of aggregates, for cases when list of aggregates is higher then batchLimit
		''' </summary>
		''' <param name="list"> </param>
		''' @param <U>
		''' @return </param>
		Public Shared Function getBatches(Of U As Aggregate)(ByVal list As IList(Of U), ByVal partitionSize As Integer) As IList(Of Batch(Of U))
			Dim c As DataType = Nothing
'JAVA TO VB CONVERTER NOTE: The variable u was renamed since Visual Basic does not allow local variables with the same name as method-level generic type parameters:
			For Each u_Conflict As val In list
				For Each a As val In u_Conflict.getArguments()
					' we'll be comparing to the first array
					If c = Nothing AndAlso a IsNot Nothing Then
						c = a.dataType()
					End If

					If a IsNot Nothing AndAlso c <> Nothing Then
						Preconditions.checkArgument(c = a.dataType(), "All arguments must have same data type")
					End If
				Next a
			Next u_Conflict

			If c = Nothing Then
				Throw New ND4JIllegalStateException("Can't infer data type from arguments")
			End If

			Dim partitions As IList(Of IList(Of U)) = Lists.partition(list, partitionSize)
			Dim split As IList(Of Batch(Of U)) = New List(Of Batch(Of U))()

			For Each partition As IList(Of U) In partitions
				split.Add(New Batch(Of U)(partition))
			Next partition

			Return split
		End Function
	End Class

End Namespace