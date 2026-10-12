Imports System
Imports System.Collections.Generic
Imports System.IO
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports PathLabelGenerator = org.datavec.api.io.labels.PathLabelGenerator
Imports PathMultiLabelGenerator = org.datavec.api.io.labels.PathMultiLabelGenerator
Imports Record = org.datavec.api.records.Record
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports LogRecordListener = org.datavec.api.records.listener.impl.LogRecordListener
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports RecordWriter = org.datavec.api.records.writer.RecordWriter
Imports CollectionInputSplit = org.datavec.api.split.CollectionInputSplit
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports NDArrayRecordBatch = org.datavec.api.writable.batch.NDArrayRecordBatch
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
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

Namespace org.datavec.image.recordreader


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class TestImageRecordReader
	Public Class TestImageRecordReader
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testEmptySplit() throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEmptySplit()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim data As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.CollectionInputSplit(New List(Of )())
			Call (New ImageRecordReader()).initialize(data, Nothing)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMetaData(@TempDir Path testDir) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMetaData(ByVal testDir As Path)

			Dim parentDir As File = testDir.toFile()
			Call (New ClassPathResource("datavec-data-image/testimages/")).copyDirectory(parentDir)
			'        System.out.println(f.getAbsolutePath());
			'        System.out.println(f.getParentFile().getParentFile().getAbsolutePath());
			Dim labelMaker As New ParentPathLabelGenerator()
			Dim rr As New ImageRecordReader(32, 32, 3, labelMaker)
			rr.initialize(New org.datavec.api.Split.FileSplit(parentDir))

			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While rr.hasNext()
				Dim l As IList(Of Writable) = rr.next()
				[out].Add(l)
				assertEquals(2, l.Count)
			Loop

			assertEquals(6, [out].Count)

			rr.reset()
			Dim out2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim out3 As IList(Of Record) = New List(Of Record)()
			Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()

			Do While rr.hasNext()
				Dim r As Record = rr.nextRecord()
				out2.Add(r.getRecord())
				out3.Add(r)
				meta.Add(r.MetaData)
				'            System.out.println(r.getMetaData() + "\t" + r.getRecord().get(1));
			Loop

			assertEquals([out], out2)

			Dim fromMeta As IList(Of Record) = rr.loadFromMetaData(meta)
			assertEquals(out3, fromMeta)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testImageRecordReaderLabelsOrder(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testImageRecordReaderLabelsOrder(ByVal testDir As Path)
			'Labels order should be consistent, regardless of file iteration order

			'Idea: labels order should be consistent regardless of input file order
			Dim f As File = testDir.toFile()
			Call (New ClassPathResource("datavec-data-image/testimages/")).copyDirectory(f)
			Dim f0 As New File(f, "/class0/0.jpg")
			Dim f1 As New File(f, "/class1/A.jpg")

			Dim order0 As IList(Of URI) = New List(Of URI) From {f0.toURI(), f1.toURI()}
			Dim order1 As IList(Of URI) = New List(Of URI) From {f1.toURI(), f0.toURI()}

			Dim labelMaker0 As New ParentPathLabelGenerator()
			Dim rr0 As New ImageRecordReader(32, 32, 3, labelMaker0)
			rr0.initialize(New org.datavec.api.Split.CollectionInputSplit(order0))

			Dim labelMaker1 As New ParentPathLabelGenerator()
			Dim rr1 As New ImageRecordReader(32, 32, 3, labelMaker1)
			rr1.initialize(New org.datavec.api.Split.CollectionInputSplit(order1))

			Dim labels0 As IList(Of String) = rr0.getLabels()
			Dim labels1 As IList(Of String) = rr1.getLabels()

			'        System.out.println(labels0);
			'        System.out.println(labels1);

			assertEquals(labels0, labels1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testImageRecordReaderRandomization(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testImageRecordReaderRandomization(ByVal testDir As Path)
			'Order of FileSplit+ImageRecordReader should be different after reset

			'Idea: labels order should be consistent regardless of input file order
			Dim f0 As File = testDir.toFile()
			Call (New ClassPathResource("datavec-data-image/testimages/")).copyDirectory(f0)

			Dim fs As New org.datavec.api.Split.FileSplit(f0, New Random(12345))

			Dim labelMaker As New ParentPathLabelGenerator()
			Dim rr As New ImageRecordReader(32, 32, 3, labelMaker)
			rr.initialize(fs)

			Dim out1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim order1 As IList(Of File) = New List(Of File)()
			Do While rr.hasNext()
				out1.Add(rr.next())
				order1.Add(rr.CurrentFile)
			Loop
			assertEquals(6, out1.Count)
			assertEquals(6, order1.Count)

			rr.reset()
			Dim out2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim order2 As IList(Of File) = New List(Of File)()
			Do While rr.hasNext()
				out2.Add(rr.next())
				order2.Add(rr.CurrentFile)
			Loop
			assertEquals(6, out2.Count)
			assertEquals(6, order2.Count)

			assertNotEquals(out1, out2)
			assertNotEquals(order1, order2)

			'Check that different seed gives different order for the initial iteration
			Dim fs2 As New org.datavec.api.Split.FileSplit(f0, New Random(999999999))

			Dim labelMaker2 As New ParentPathLabelGenerator()
			Dim rr2 As New ImageRecordReader(32, 32, 3, labelMaker2)
			rr2.initialize(fs2)

			Dim order3 As IList(Of File) = New List(Of File)()
			Do While rr2.hasNext()
				rr2.next()
				order3.Add(rr2.CurrentFile)
			Loop
			assertEquals(6, order3.Count)

			assertNotEquals(order1, order3)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testImageRecordReaderRegression(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testImageRecordReaderRegression(ByVal testDir As Path)

			Dim regressionLabelGen As PathLabelGenerator = New TestRegressionLabelGen()

			Dim rr As New ImageRecordReader(28, 28, 3, regressionLabelGen)

			Dim rootDir As File = testDir.toFile()
			Call (New ClassPathResource("datavec-data-image/testimages/")).copyDirectory(rootDir)
			Dim fs As New org.datavec.api.Split.FileSplit(rootDir)
			rr.initialize(fs)
			Dim arr() As URI = fs.locations()

			assertTrue(rr.getLabels() Is Nothing OrElse rr.getLabels().Count = 0)

			Dim expLabels As IList(Of Writable) = New List(Of Writable)()
			For Each u As URI In arr
				Dim path As String = u.getPath()
								Dim tempVar = path.length()-5
				expLabels.Add(testLabel(path.Substring(tempVar, path.Length - (tempVar))))
			Next u

			Dim count As Integer = 0
			Do While rr.hasNext()
				Dim l As IList(Of Writable) = rr.next()

				assertEquals(2, l.Count)
				assertEquals(expLabels(count), l(1))

				count += 1
			Loop
			assertEquals(6, count)

			'Test batch ops:
			rr.reset()

			Dim b1 As IList(Of IList(Of Writable)) = rr.next(3)
			Dim b2 As IList(Of IList(Of Writable)) = rr.next(3)
			assertFalse(rr.hasNext())

			Dim b1a As NDArrayRecordBatch = CType(b1, NDArrayRecordBatch)
			Dim b2a As NDArrayRecordBatch = CType(b2, NDArrayRecordBatch)
			assertEquals(2, b1a.getArrays().size())
			assertEquals(2, b2a.getArrays().size())

			Dim l1 As New NDArrayWritable(Nd4j.create(New Double(){expLabels(0).toDouble(), expLabels(1).toDouble(), expLabels(2).toDouble()}, New Long(){3, 1}, DataType.FLOAT))
			Dim l2 As New NDArrayWritable(Nd4j.create(New Double(){expLabels(3).toDouble(), expLabels(4).toDouble(), expLabels(5).toDouble()}, New Long(){3, 1}, DataType.FLOAT))

			Dim act1 As INDArray = b1a.getArrays().get(1)
			Dim act2 As INDArray = b2a.getArrays().get(1)
			assertEquals(l1.get(), act1)
			assertEquals(l2.get(), act2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListenerInvocationBatch(@TempDir Path testDir) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testListenerInvocationBatch(ByVal testDir As Path)
			Dim labelMaker As New ParentPathLabelGenerator()
			Dim rr As New ImageRecordReader(32, 32, 3, labelMaker)
			Dim f As File = testDir.toFile()
			Call (New ClassPathResource("datavec-data-image/testimages/")).copyDirectory(f)

			Dim parent As File = f
			Dim numFiles As Integer = 6
			rr.initialize(New org.datavec.api.Split.FileSplit(parent))
			Dim counting As New CountingListener(New LogRecordListener())
			rr.setListeners(counting)
			rr.next(numFiles + 1)
			assertEquals(numFiles, counting.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListenerInvocationSingle(@TempDir Path testDir) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testListenerInvocationSingle(ByVal testDir As Path)
			Dim labelMaker As New ParentPathLabelGenerator()
			Dim rr As New ImageRecordReader(32, 32, 3, labelMaker)
			Dim parent As File = testDir.toFile()
			Call (New ClassPathResource("datavec-data-image/testimages/class0/")).copyDirectory(parent)
			Dim numFiles As Integer = parent.list().length
			rr.initialize(New org.datavec.api.Split.FileSplit(parent))
			Dim counting As New CountingListener(New LogRecordListener())
			rr.setListeners(counting)
			Do While rr.hasNext()
				rr.next()
			Loop
			assertEquals(numFiles, counting.Count)
		End Sub

		<Serializable>
		Private Class TestRegressionLabelGen
			Implements PathLabelGenerator

			Public Overridable Function getLabelForPath(ByVal path As String) As Writable Implements PathLabelGenerator.getLabelForPath
								Dim tempVar = path.length()-5
				Dim filename As String = path.Substring(tempVar, path.Length - (tempVar))
				Return testLabel(filename)
			End Function

			Public Overridable Function getLabelForPath(ByVal uri As URI) As Writable Implements PathLabelGenerator.getLabelForPath
				Return getLabelForPath(uri.ToString())
			End Function

			Public Overridable Function inferLabelClasses() As Boolean Implements PathLabelGenerator.inferLabelClasses
				Return False
			End Function
		End Class

		Private Shared Function testLabel(ByVal filename As String) As Writable
			Select Case filename
				Case "0.jpg"
					Return New DoubleWritable(0.0)
				Case "1.png"
					Return New DoubleWritable(1.0)
				Case "2.jpg"
					Return New DoubleWritable(2.0)
				Case "A.jpg"
					Return New DoubleWritable(10)
				Case "B.png"
					Return New DoubleWritable(11)
				Case "C.jpg"
					Return New DoubleWritable(12)
				Case Else
					Throw New Exception(filename)
			End Select
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testImageRecordReaderPathMultiLabelGenerator(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testImageRecordReaderPathMultiLabelGenerator(ByVal testDir As Path)
			Nd4j.DataType = DataType.FLOAT
			'Assumption: 2 multi-class (one hot) classification labels: 2 and 3 classes respectively
			' PLUS single value (Writable) regression label

			Dim multiLabelGen As PathMultiLabelGenerator = New TestPathMultiLabelGenerator()

			Dim rr As New ImageRecordReader(28, 28, 3, multiLabelGen)

			Dim rootDir As File = testDir.toFile()
			Call (New ClassPathResource("datavec-data-image/testimages/")).copyDirectory(rootDir)
			Dim fs As New org.datavec.api.Split.FileSplit(rootDir)
			rr.initialize(fs)
			Dim arr() As URI = fs.locations()

			assertTrue(rr.getLabels() Is Nothing OrElse rr.getLabels().Count = 0)

			Dim expLabels As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For Each u As URI In arr
				Dim path As String = u.getPath()
								Dim tempVar = path.length()-5
				expLabels.Add(testMultiLabel(path.Substring(tempVar, path.Length - (tempVar))))
			Next u

			Dim count As Integer = 0
			Do While rr.hasNext()
				Dim l As IList(Of Writable) = rr.next()
				assertEquals(4, l.Count)
				For i As Integer = 0 To 2
					assertEquals(expLabels(count)(i), l(i+1))
				Next i
				count += 1
			Loop
			assertEquals(6, count)

			'Test batch ops:
			rr.reset()
			Dim b1 As IList(Of IList(Of Writable)) = rr.next(3)
			Dim b2 As IList(Of IList(Of Writable)) = rr.next(3)
			assertFalse(rr.hasNext())

			Dim b1a As NDArrayRecordBatch = CType(b1, NDArrayRecordBatch)
			Dim b2a As NDArrayRecordBatch = CType(b2, NDArrayRecordBatch)
			assertEquals(4, b1a.getArrays().size())
			assertEquals(4, b2a.getArrays().size())

			Dim l1a As New NDArrayWritable(Nd4j.vstack(CType(expLabels(0)(0), NDArrayWritable).get(), CType(expLabels(1)(0), NDArrayWritable).get(), CType(expLabels(2)(0), NDArrayWritable).get()))
			Dim l1b As New NDArrayWritable(Nd4j.vstack(CType(expLabels(0)(1), NDArrayWritable).get(), CType(expLabels(1)(1), NDArrayWritable).get(), CType(expLabels(2)(1), NDArrayWritable).get()))
			Dim l1c As New NDArrayWritable(Nd4j.create(New Double(){ expLabels(0)(2).toDouble(), expLabels(1)(2).toDouble(), expLabels(2)(2).toDouble()}, New Long(){1, 3}, DataType.FLOAT))


			Dim l2a As New NDArrayWritable(Nd4j.vstack(CType(expLabels(3)(0), NDArrayWritable).get(), CType(expLabels(4)(0), NDArrayWritable).get(), CType(expLabels(5)(0), NDArrayWritable).get()))
			Dim l2b As New NDArrayWritable(Nd4j.vstack(CType(expLabels(3)(1), NDArrayWritable).get(), CType(expLabels(4)(1), NDArrayWritable).get(), CType(expLabels(5)(1), NDArrayWritable).get()))
			Dim l2c As New NDArrayWritable(Nd4j.create(New Double(){ expLabels(3)(2).toDouble(), expLabels(4)(2).toDouble(), expLabels(5)(2).toDouble()}, New Long(){1, 3}, DataType.FLOAT))



			assertEquals(l1a.get(), b1a.getArrays().get(1))
			assertEquals(l1b.get(), b1a.getArrays().get(2))
			assertEquals(l1c.get(), b1a.getArrays().get(3))

			assertEquals(l2a.get(), b2a.getArrays().get(1))
			assertEquals(l2b.get(), b2a.getArrays().get(2))
			assertEquals(l2c.get(), b2a.getArrays().get(3))
		End Sub

		<Serializable>
		Private Class TestPathMultiLabelGenerator
			Implements PathMultiLabelGenerator

			Public Overridable Function getLabels(ByVal uriPath As String) As IList(Of Writable) Implements PathMultiLabelGenerator.getLabels
				Dim filename As String = uriPath.Substring(uriPath.Length-5)
				Return testMultiLabel(filename)
			End Function
		End Class

		Private Shared Function testMultiLabel(ByVal filename As String) As IList(Of Writable)
			Select Case filename
				Case "0.jpg"
					Return New List(Of Writable) From {Of Writable}
				Case "1.png"
					Return New List(Of Writable) From {Of Writable}
				Case "2.jpg"
					Return New List(Of Writable) From {Of Writable}
				Case "A.jpg"
					Return New List(Of Writable) From {Of Writable}
				Case "B.png"
					Return New List(Of Writable) From {Of Writable}
				Case "C.jpg"
					Return New List(Of Writable) From {Of Writable}
				Case Else
					Throw New Exception(filename)
			End Select
		End Function

		<Serializable>
		Private Class CountingListener
			Implements RecordListener

			Friend listener As RecordListener
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer = 0

			Public Sub New(ByVal listener As RecordListener)
				Me.listener = listener
			End Sub

			Public Overridable Function invoked() As Boolean Implements RecordListener.invoked
				Return Me.listener.invoked()
			End Function

			Public Overridable Sub invoke() Implements RecordListener.invoke
				Me.listener.invoke()
			End Sub

			Public Overridable Sub recordRead(ByVal reader As RecordReader, ByVal record As Object) Implements RecordListener.recordRead
				Me.listener.recordRead(reader, record)
				Me.count_Conflict += 1
			End Sub

			Public Overridable Sub recordWrite(ByVal writer As RecordWriter, ByVal record As Object) Implements RecordListener.recordWrite
				Me.listener.recordWrite(writer, record)
				Me.count_Conflict += 1
			End Sub

			Public Overridable ReadOnly Property Count As Integer
				Get
					Return count_Conflict
				End Get
			End Property
		End Class



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNCHW_NCHW(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNCHW_NCHW(ByVal testDir As Path)
			'Idea: labels order should be consistent regardless of input file order
			Dim f0 As File = testDir.toFile()
			Call (New ClassPathResource("datavec-data-image/testimages/")).copyDirectory(f0)

			Dim fs0 As New org.datavec.api.Split.FileSplit(f0, New Random(12345))
			Dim fs1 As New org.datavec.api.Split.FileSplit(f0, New Random(12345))
			assertEquals(6, fs0.locations().Length)
			assertEquals(6, fs1.locations().Length)

			Dim nchw As New ImageRecordReader(32, 32, 3, True)
			nchw.initialize(fs0)

			Dim nhwc As New ImageRecordReader(32, 32, 3, False)
			nhwc.initialize(fs1)

			Do While nchw.hasNext()
				assertTrue(nhwc.hasNext())

				Dim l_nchw As IList(Of Writable) = nchw.next()
				Dim l_nhwc As IList(Of Writable) = nhwc.next()

				Dim a_nchw As INDArray = DirectCast(l_nchw(0), NDArrayWritable).get()
				Dim a_nhwc As INDArray = DirectCast(l_nhwc(0), NDArrayWritable).get()

				assertArrayEquals(New Long(){1, 3, 32, 32}, a_nchw.shape())
				assertArrayEquals(New Long(){1, 32, 32, 3}, a_nhwc.shape())

				Dim permuted As INDArray = a_nhwc.permute(0,3,1,2) 'NHWC to NCHW
				assertEquals(a_nchw, permuted)
			Loop


			'Test batch:
			nchw.reset()
			nhwc.reset()

			Dim batchCount As Integer = 0
			Do While nchw.hasNext()
				assertTrue(nhwc.hasNext())
				batchCount += 1

				Dim l_nchw As IList(Of IList(Of Writable)) = nchw.next(3)
				Dim l_nhwc As IList(Of IList(Of Writable)) = nhwc.next(3)
				assertEquals(3, l_nchw.Count)
				assertEquals(3, l_nhwc.Count)

				Dim b_nchw As NDArrayRecordBatch = CType(l_nchw, NDArrayRecordBatch)
				Dim b_nhwc As NDArrayRecordBatch = CType(l_nhwc, NDArrayRecordBatch)

				Dim a_nchw As INDArray = b_nchw.getArrays().get(0)
				Dim a_nhwc As INDArray = b_nhwc.getArrays().get(0)

				assertArrayEquals(New Long(){3, 3, 32, 32}, a_nchw.shape())
				assertArrayEquals(New Long(){3, 32, 32, 3}, a_nhwc.shape())

				Dim permuted As INDArray = a_nhwc.permute(0,3,1,2) 'NHWC to NCHW
				assertEquals(a_nchw, permuted)
			Loop
			assertEquals(2, batchCount)


			'Test record(URI, DataInputStream)

			Dim u As URI = fs0.locations()(0)

			Using dis As New DataInputStream(New BufferedInputStream(New FileStream(u, FileMode.Open, FileAccess.Read)))
				Dim l As IList(Of Writable) = nchw.record(u, dis)
				Dim arr As INDArray = DirectCast(l(0), NDArrayWritable).get()
				assertArrayEquals(New Long(){1, 3, 32, 32}, arr.shape())
			End Using

			Using dis As New DataInputStream(New BufferedInputStream(New FileStream(u, FileMode.Open, FileAccess.Read)))
				Dim l As IList(Of Writable) = nhwc.record(u, dis)
				Dim arr As INDArray = DirectCast(l(0), NDArrayWritable).get()
				assertArrayEquals(New Long(){1, 32, 32, 3}, arr.shape())
			End Using
		End Sub
	End Class


End Namespace