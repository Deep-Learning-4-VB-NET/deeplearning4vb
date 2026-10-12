Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DataSetLoaderIterator = org.deeplearning4j.datasets.iterator.loader.DataSetLoaderIterator
Imports MultiDataSetLoaderIterator = org.deeplearning4j.datasets.iterator.loader.MultiDataSetLoaderIterator
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports org.nd4j.common.loader
Imports LocalFileSourceFactory = org.nd4j.common.loader.LocalFileSourceFactory
Imports Source = org.nd4j.common.loader.Source
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class LoaderIteratorTests extends org.deeplearning4j.BaseDL4JTest
	Public Class LoaderIteratorTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDSLoaderIter()
		Public Overridable Sub testDSLoaderIter()

			For Each r As Boolean In New Boolean(){False, True}
				Dim l As IList(Of String) = New List(Of String) From {"3", "0", "1"}
				Dim rng As Random = If(r, New Random(12345), Nothing)
				Dim iter As DataSetIterator = New DataSetLoaderIterator(l, rng, New LoaderAnonymousInnerClass(Me)
			   , New LocalFileSourceFactory())

				Dim count As Integer = 0
				Dim exp() As Integer = {3, 0, 1}
				Do While iter.MoveNext()
					Dim ds As DataSet = iter.Current
					If Not r Then
						assertEquals(exp(count), ds.Features.getInt(0))
					End If
					count += 1
				Loop
				assertEquals(3, count)

				iter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertTrue(iter.hasNext())
			Next r
		End Sub

		Private Class LoaderAnonymousInnerClass
			Implements Loader(Of DataSet)

			Private ReadOnly outerInstance As LoaderIteratorTests

			Public Sub New(ByVal outerInstance As LoaderIteratorTests)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.DataSet load(org.nd4j.common.loader.Source source) throws java.io.IOException
			Public Function load(ByVal source As Source) As DataSet
				Dim i As INDArray = Nd4j.scalar(Convert.ToInt32(source.Path))
				Return New DataSet(i, i)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMDSLoaderIter()
		Public Overridable Sub testMDSLoaderIter()

			For Each r As Boolean In New Boolean(){False, True}
				Dim l As IList(Of String) = New List(Of String) From {"3", "0", "1"}
				Dim rng As Random = If(r, New Random(12345), Nothing)
				Dim iter As MultiDataSetIterator = New MultiDataSetLoaderIterator(l, Nothing, New LoaderAnonymousInnerClass2(Me)
			   , New LocalFileSourceFactory())

				Dim count As Integer = 0
				Dim exp() As Integer = {3, 0, 1}
				Do While iter.MoveNext()
					Dim ds As MultiDataSet = iter.Current
					If Not r Then
						assertEquals(exp(count), ds.Features(0).getInt(0))
					End If
					count += 1
				Loop
				assertEquals(3, count)

				iter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertTrue(iter.hasNext())
			Next r
		End Sub

		Private Class LoaderAnonymousInnerClass2
			Implements Loader(Of MultiDataSet)

			Private ReadOnly outerInstance As LoaderIteratorTests

			Public Sub New(ByVal outerInstance As LoaderIteratorTests)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.api.MultiDataSet load(org.nd4j.common.loader.Source source) throws java.io.IOException
			Public Function load(ByVal source As Source) As MultiDataSet
				Dim i As INDArray = Nd4j.scalar(Convert.ToInt32(source.Path))
				Return New org.nd4j.linalg.dataset.MultiDataSet(i, i)
			End Function
		End Class

	End Class

End Namespace