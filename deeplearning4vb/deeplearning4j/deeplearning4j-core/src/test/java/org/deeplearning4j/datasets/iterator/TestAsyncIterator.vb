Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.datasets.iterator



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.NDARRAY_ETL) public class TestAsyncIterator extends org.deeplearning4j.BaseDL4JTest
	Public Class TestAsyncIterator
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasic()
		Public Overridable Sub testBasic()

			'Basic test. Make sure it returns the right number of elements,
			' hasNext() works, etc

			Dim size As Integer = 13

			Dim baseIter As DataSetIterator = New TestIterator(size, 0)

			'async iterator with queue size of 1
			Dim async As DataSetIterator = New AsyncDataSetIterator(baseIter, 1)

			For i As Integer = 0 To size - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertTrue(async.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = async.next()
				assertEquals(ds.Features.getDouble(0), i, 0.0)
				assertEquals(ds.Labels.getDouble(0), i, 0.0)
			Next i

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(async.hasNext())
			async.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(async.hasNext())
			DirectCast(async, AsyncDataSetIterator).shutdown()

			'async iterator with queue size of 5
			baseIter = New TestIterator(size, 5)
			async = New AsyncDataSetIterator(baseIter, 5)

			For i As Integer = 0 To size - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertTrue(async.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = async.next()
				assertEquals(ds.Features.getDouble(0), i, 0.0)
				assertEquals(ds.Labels.getDouble(0), i, 0.0)
			Next i
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(async.hasNext())
			async.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(async.hasNext())
			DirectCast(async, AsyncDataSetIterator).shutdown()

			'async iterator with queue size of 100
			baseIter = New TestIterator(size, 100)
			async = New AsyncDataSetIterator(baseIter, 100)

			For i As Integer = 0 To size - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertTrue(async.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = async.next()
				Do While ds Is Nothing
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					ds = async.next()
				Loop
				assertEquals(ds.Features.getDouble(0), i, 0.0)
				assertEquals(ds.Labels.getDouble(0), i, 0.0)
			Next i

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(async.hasNext())
			async.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(async.hasNext())
			DirectCast(async, AsyncDataSetIterator).shutdown()

			'Test iteration where performance is limited by baseIterator.next() speed
			baseIter = New TestIterator(size, 1000)
			async = New AsyncDataSetIterator(baseIter, 5)
			For i As Integer = 0 To size - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertTrue(async.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = async.next()
				assertEquals(ds.Features.getDouble(0), i, 0.0)
				assertEquals(ds.Labels.getDouble(0), i, 0.0)
			Next i
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(async.hasNext())
			async.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(async.hasNext())
			DirectCast(async, AsyncDataSetIterator).shutdown()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitializeNoNextIter()
		Public Overridable Sub testInitializeNoNextIter()

			Dim iter As DataSetIterator = New IrisDataSetIterator(10, 150)
			Do While iter.MoveNext()
				iter.Current
			Loop

			Dim async As DataSetIterator = New AsyncDataSetIterator(iter, 2)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(iter.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(async.hasNext())
			Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				iter.next()
				fail("Should have thrown NoSuchElementException")
			Catch e As Exception
				'OK
			End Try

			async.reset()
			Dim count As Integer = 0
			Do While async.MoveNext()
				async.Current
				count += 1
			Loop
			assertEquals(150 \ 10, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testResetWhileBlocking()
		Public Overridable Sub testResetWhileBlocking()
			Dim size As Integer = 6
			'Test reset while blocking on baseIterator.next()
			Dim baseIter As DataSetIterator = New TestIterator(size, 1000)
			Dim async As New AsyncDataSetIterator(baseIter)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			async.next()
			'Should be waiting on baseIter.next()
			async.reset()
			For i As Integer = 0 To 5
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertTrue(async.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = async.next()
				assertEquals(ds.Features.getDouble(0), i, 0.0)
				assertEquals(ds.Labels.getDouble(0), i, 0.0)
			Next i
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(async.hasNext())
			async.shutdown()

			'Test reset while blocking on blockingQueue.put()
			baseIter = New TestIterator(size, 0)
			async = New AsyncDataSetIterator(baseIter)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			async.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			async.next()
			'Should be waiting on blocingQueue
			async.reset()
			For i As Integer = 0 To 5
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertTrue(async.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = async.next()
				assertEquals(ds.Features.getDouble(0), i, 0.0)
				assertEquals(ds.Labels.getDouble(0), i, 0.0)
			Next i
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(async.hasNext())
			async.shutdown()
		End Sub


		<Serializable>
		Private Class TestIterator
			Implements DataSetIterator

			Friend size As Integer
			Friend cursor As Integer
			Friend delayMSOnNext As Long

			Friend Sub New(ByVal size As Integer, ByVal delayMSOnNext As Long)
				Me.size = size
				Me.cursor = 0
				Me.delayMSOnNext = delayMSOnNext
			End Sub

			Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
				Throw New System.NotSupportedException()
			End Function

			Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
				Return 1
			End Function

			Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
				Return 1
			End Function

			Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
				Return True
			End Function

			Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
				Return False
			End Function

			Public Overridable Sub reset() Implements DataSetIterator.reset
				cursor = 0
			End Sub

			Public Overridable Function batch() As Integer Implements DataSetIterator.batch
				Return 1
			End Function

			Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
				Set(ByVal preProcessor As DataSetPreProcessor)
					Throw New System.NotSupportedException()
				End Set
				Get
					Throw New System.NotSupportedException()
				End Get
			End Property


			Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides Function hasNext() As Boolean
				Return cursor < size
			End Function

			Public Overrides Function [next]() As DataSet
				If delayMSOnNext > 0 Then
					Try
						Thread.Sleep(delayMSOnNext)
					Catch e As InterruptedException
						Throw New Exception(e)
					End Try
				End If
				Dim features As INDArray = Nd4j.scalar(cursor)
				Dim labels As INDArray = Nd4j.scalar(cursor)
				cursor += 1
				Return New DataSet(features, labels)
			End Function

			Public Overrides Sub remove()
			End Sub
		End Class

	End Class

End Namespace