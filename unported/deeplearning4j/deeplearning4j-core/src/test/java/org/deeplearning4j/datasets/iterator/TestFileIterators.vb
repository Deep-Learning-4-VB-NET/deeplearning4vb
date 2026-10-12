Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports FileDataSetIterator = org.deeplearning4j.datasets.iterator.file.FileDataSetIterator
Imports FileMultiDataSetIterator = org.deeplearning4j.datasets.iterator.file.FileMultiDataSetIterator
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals

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
'ORIGINAL LINE: @Disabled @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.NDARRAY_ETL) public class TestFileIterators extends org.deeplearning4j.BaseDL4JTest
	Public Class TestFileIterators
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFileDataSetIterator(@TempDir Path folder, @TempDir Path testDir2) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFileDataSetIterator(ByVal folder As Path, ByVal testDir2 As Path)

			Dim f As File = folder.toFile()

			Dim d1 As New DataSet(Nd4j.linspace(1, 10, 10).reshape(ChrW(10), 1), Nd4j.linspace(101, 110, 10).reshape(ChrW(10), 1))
			Dim d2 As New DataSet(Nd4j.linspace(11, 20, 10).reshape(ChrW(10), 1), Nd4j.linspace(111, 120, 10).reshape(ChrW(10), 1))
			Dim d3 As New DataSet(Nd4j.linspace(21, 30, 10).reshape(ChrW(10), 1), Nd4j.linspace(121, 130, 10).reshape(ChrW(10), 1))

			d1.save(New File(f, "d1.bin"))
			Dim f2 As New File(f, "subdir/d2.bin")
			f2.getParentFile().mkdir()
			d2.save(f2)
			d3.save(New File(f, "d3.otherExt"))

			Dim exp As IDictionary(Of Double, DataSet) = New Dictionary(Of Double, DataSet)()
			exp(d1.Features.getDouble(0)) = d1
			exp(d2.Features.getDouble(0)) = d2
			exp(d3.Features.getDouble(0)) = d3
			Dim iter As DataSetIterator = New FileDataSetIterator(f, True, Nothing, -1, DirectCast(Nothing, String()))
			Dim act As IDictionary(Of Double, DataSet) = New Dictionary(Of Double, DataSet)()
			Do While iter.MoveNext()
				Dim d As DataSet = iter.Current
				act(d.Features.getDouble(0)) = d
			Loop
			assertEquals(exp, act)

			'Test multiple directories

			Dim f2a As New File(testDir2.toFile(),"folder1")
			f2a.mkdirs()
			Dim f2b As New File(testDir2.toFile(),"folder2")
			f2b.mkdirs()
			Dim f2c As New File(testDir2.toFile(),"folder3")
			f2c.mkdirs()
			d1.save(New File(f2a, "d1.bin"))
			d2.save(New File(f2a, "d2.bin"))
			d3.save(New File(f2b, "d3.bin"))

			d1.save(New File(f2c, "d1.bin"))
			d2.save(New File(f2c, "d2.bin"))
			d3.save(New File(f2c, "d3.bin"))
			iter = New FileDataSetIterator(f2c, True, Nothing, -1, DirectCast(Nothing, String()))
			Dim iterMultiDir As DataSetIterator = New FileDataSetIterator(New File(){f2a, f2b}, True, Nothing, -1, DirectCast(Nothing, String()))

			iter.reset()
			Dim count As Integer = 0
			Dim iter1Out As IDictionary(Of Double, DataSet) = New Dictionary(Of Double, DataSet)()
			Dim iter2Out As IDictionary(Of Double, DataSet) = New Dictionary(Of Double, DataSet)()
			Do While iter.MoveNext()
				Dim ds1 As DataSet = iter.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds2 As DataSet = iterMultiDir.next()
				'assertEquals(ds1, ds2);   //Iteration order may not be consistent across all platforms due to file listing order differences
				iter1Out(ds1.Features.getDouble(0)) = ds1
				iter2Out(ds2.Features.getDouble(0)) = ds2
				count += 1
			Loop
			assertEquals(3, count)
			assertEquals(iter1Out, iter2Out)



			'Test with extension filtering:
			exp.Clear()
			exp(d1.Features.getDouble(0)) = d1
			exp(d2.Features.getDouble(0)) = d2
			iter = New FileDataSetIterator(f, True, Nothing, -1, "bin")
			act.Clear()
			Do While iter.MoveNext()
				Dim d As DataSet = iter.Current
				act(d.Features.getDouble(0)) = d
			Loop
			assertEquals(exp, act)

			'Test non-recursive
			exp.Clear()
			exp(d1.Features.getDouble(0)) = d1
			exp(d3.Features.getDouble(0)) = d3
			iter = New FileDataSetIterator(f, False, Nothing, -1, DirectCast(Nothing, String()))
			act.Clear()
			Do While iter.MoveNext()
				Dim d As DataSet = iter.Current
				act(d.Features.getDouble(0)) = d
			Loop
			assertEquals(exp, act)


			'Test batch size != saved size
			Dim f4 As New File(folder.toFile(),"newFolder")
			f4.mkdirs()
			f = f4
			d1.save(New File(f, "d1.bin"))
			d2.save(New File(f, "d2.bin"))
			d3.save(New File(f, "d3.bin"))
	'        
	'        //TODO different file iteration orders make the batch recombining hard to test...
	'        exp = Arrays.asList(
	'                new DataSet(Nd4j.linspace(1, 15, 15).reshape(10,1),
	'                        Nd4j.linspace(101, 115, 15).reshape(10,1)),
	'                new DataSet(Nd4j.linspace(16, 30, 15).reshape(10,1),
	'                        Nd4j.linspace(116, 130, 15).reshape(10,1)));
	'        act = new ArrayList<>();
	'        
			iter = New FileDataSetIterator(f, True, Nothing, 15, DirectCast(Nothing, String()))
			count = 0
			Do While iter.MoveNext()
				Dim [next] As DataSet = iter.Current
				assertArrayEquals(New Long(){15, 1}, [next].Features.shape())
				assertArrayEquals(New Long(){15, 1}, [next].Labels.shape())
				count += 1
			Loop
			assertEquals(2, count) '2x15 = 30 examples
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFileMultiDataSetIterator(@TempDir Path folder) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFileMultiDataSetIterator(ByVal folder As Path)
			Dim f As File = folder.toFile()

			Dim d1 As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(Nd4j.linspace(1, 10, 10).reshape(ChrW(10), 1), Nd4j.linspace(101, 110, 10).reshape(ChrW(10), 1))
			Dim d2 As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(Nd4j.linspace(11, 20, 10).reshape(ChrW(10), 1), Nd4j.linspace(111, 120, 10).reshape(ChrW(10), 1))
			Dim d3 As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(Nd4j.linspace(21, 30, 10).reshape(ChrW(10), 1), Nd4j.linspace(121, 130, 10).reshape(ChrW(10), 1))

			d1.save(New File(f, "d1.bin"))
			Dim f2 As New File(f, "subdir/d2.bin")
			f2.getParentFile().mkdir()
			d2.save(f2)
			d3.save(New File(f, "d3.otherExt"))

			Dim exp As IDictionary(Of Double, MultiDataSet) = New Dictionary(Of Double, MultiDataSet)()
			exp(d1.getFeatures(0).getDouble(0)) = d1
			exp(d2.getFeatures(0).getDouble(0)) = d2
			exp(d3.getFeatures(0).getDouble(0)) = d3
			Dim iter As MultiDataSetIterator = New FileMultiDataSetIterator(f, True, Nothing, -1, DirectCast(Nothing, String()))
			Dim act As IDictionary(Of Double, MultiDataSet) = New Dictionary(Of Double, MultiDataSet)()
			Do While iter.MoveNext()
				Dim [next] As MultiDataSet = iter.Current
				act([next].getFeatures(0).getDouble(0)) = [next]
			Loop
			assertEquals(exp, act)

			'Test multiple directories
			Dim newDir As New File(folder.toFile(),"folder2")
			newDir.mkdirs()
			Dim f2a As New File(newDir,"folder-1")
			Dim f2b As New File(newDir,"folder-2")
			Dim f2c As New File(newDir,"folder-3")
			d1.save(New File(f2a, "d1.bin"))
			d2.save(New File(f2a, "d2.bin"))
			d3.save(New File(f2b, "d3.bin"))

			d1.save(New File(f2c, "d1.bin"))
			d2.save(New File(f2c, "d2.bin"))
			d3.save(New File(f2c, "d3.bin"))
			iter = New FileMultiDataSetIterator(f2c, True, Nothing, -1, DirectCast(Nothing, String()))
			Dim iterMultiDir As MultiDataSetIterator = New FileMultiDataSetIterator(New File(){f2a, f2b}, True, Nothing, -1, DirectCast(Nothing, String()))

			iter.reset()
			Dim count As Integer = 0
			Dim m1 As IDictionary(Of Double, MultiDataSet) = New Dictionary(Of Double, MultiDataSet)() 'Use maps due to possibility of file iteration order differing on some platforms
			Dim m2 As IDictionary(Of Double, MultiDataSet) = New Dictionary(Of Double, MultiDataSet)()
			Do While iter.MoveNext()
				Dim ds1 As MultiDataSet = iter.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds2 As MultiDataSet = iterMultiDir.next()
				m1(ds1.getFeatures(0).getDouble(0)) = ds1
				m2(ds2.getFeatures(0).getDouble(0)) = ds2
				count += 1
			Loop
			assertEquals(3, count)
			assertEquals(m1, m2)

			'Test with extension filtering:
			exp = New Dictionary(Of Double, MultiDataSet)()
			exp(d1.getFeatures(0).getDouble(0)) = d1
			exp(d2.getFeatures(0).getDouble(0)) = d2
			iter = New FileMultiDataSetIterator(f, True, Nothing, -1, "bin")
			act = New Dictionary(Of Double, MultiDataSet)()
			Do While iter.MoveNext()
				Dim [next] As MultiDataSet = iter.Current
				act([next].getFeatures(0).getDouble(0)) = [next]
			Loop
			assertEquals(exp, act)

			'Test non-recursive
			exp = New Dictionary(Of Double, MultiDataSet)()
			exp(d1.getFeatures(0).getDouble(0)) = d1
			exp(d3.getFeatures(0).getDouble(0)) = d3
			iter = New FileMultiDataSetIterator(f, False, Nothing, -1, DirectCast(Nothing, String()))
			act = New Dictionary(Of Double, MultiDataSet)()
			Do While iter.MoveNext()
				Dim [next] As MultiDataSet = iter.Current
				act([next].getFeatures(0).getDouble(0)) = [next]
			Loop
			assertEquals(exp, act)


			'Test batch size != saved size
			f = New File(folder.toFile(),"newolder")
			f.mkdirs()
			d1.save(New File(f, "d1.bin"))
			d2.save(New File(f, "d2.bin"))
			d3.save(New File(f, "d3.bin"))
	'        
	'        //TODO different file iteration orders make the batch recombining hard to test...
	'        exp = Arrays.<MultiDataSet>asList(
	'                new org.nd4j.linalg.dataset.MultiDataSet(Nd4j.linspace(1, 15, 15).reshape(10,1),
	'                        Nd4j.linspace(101, 115, 15).reshape(10,1)),
	'                new org.nd4j.linalg.dataset.MultiDataSet(Nd4j.linspace(16, 30, 15).reshape(10,1),
	'                        Nd4j.linspace(116, 130, 15).reshape(10,1)));
	'        act = new ArrayList<>();
	'        while (iter.hasNext()) {
	'            act.add(iter.next());
	'        }
	'        assertEquals(exp, act);
	'        
			iter = New FileMultiDataSetIterator(f, True, Nothing, 15, DirectCast(Nothing, String()))
			count = 0
			Do While iter.MoveNext()
				Dim [next] As MultiDataSet = iter.Current
				assertArrayEquals(New Long(){15, 1}, [next].getFeatures(0).shape())
				assertArrayEquals(New Long(){15, 1}, [next].getLabels(0).shape())
				count += 1
			Loop
			assertEquals(2, count) '2x15 = 30 examples
		End Sub

	End Class

End Namespace