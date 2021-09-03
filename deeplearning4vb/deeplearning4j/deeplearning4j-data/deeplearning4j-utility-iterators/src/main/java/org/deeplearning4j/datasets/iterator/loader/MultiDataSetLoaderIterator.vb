Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports org.nd4j.common.loader
Imports Source = org.nd4j.common.loader.Source
Imports SourceFactory = org.nd4j.common.loader.SourceFactory
Imports LocalFileSourceFactory = org.nd4j.common.loader.LocalFileSourceFactory
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports MathUtils = org.nd4j.common.util.MathUtils

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

Namespace org.deeplearning4j.datasets.iterator.loader


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class MultiDataSetLoaderIterator implements org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
	<Serializable>
	Public Class MultiDataSetLoaderIterator
		Implements MultiDataSetIterator

		Protected Friend ReadOnly paths As IList(Of String)
		Protected Friend ReadOnly iter As IEnumerator(Of String)
		Protected Friend ReadOnly loader As Loader(Of MultiDataSet)
		Protected Friend ReadOnly sourceFactory As SourceFactory
		Protected Friend ReadOnly rng As Random
		Protected Friend ReadOnly order() As Integer
		Protected Friend preProcessor As MultiDataSetPreProcessor
		Protected Friend position As Integer

		''' <summary>
		''' NOTE: When using this constructor (with {@code Iterator<String>}) the MultiDataSetIterator cannot be reset.
		''' Use the other construtor that takes {@code Collection<String>}
		''' </summary>
		''' <param name="paths">         Paths to iterate over </param>
		''' <param name="loader">        Loader to use when loading DataSets </param>
		''' <param name="sourceFactory"> The factory to use to convert the paths into streams via <seealso cref="Source"/> </param>
		Public Sub New(ByVal paths As IEnumerator(Of String), ByVal loader As Loader(Of MultiDataSet), ByVal sourceFactory As SourceFactory)
			Me.paths = Nothing
			Me.iter = paths
			Me.loader = loader
			Me.sourceFactory = sourceFactory
			Me.rng = Nothing
			Me.order = Nothing
		End Sub

		''' <summary>
		''' Iterate of the specified collection of strings without randomization
		''' </summary>
		''' <param name="paths">         Paths to iterate over </param>
		''' <param name="loader">        Loader to use when loading DataSets </param>
		''' <param name="sourceFactory"> The factory to use to convert the paths into streams via <seealso cref="Source"/> </param>
		Public Sub New(ByVal paths As ICollection(Of String), ByVal loader As Loader(Of MultiDataSet), ByVal sourceFactory As SourceFactory)
			Me.New(paths, Nothing, loader, sourceFactory)
		End Sub

		''' <summary>
		''' Iterate of the specified collection of strings with optional randomization
		''' </summary>
		''' <param name="paths">         Paths to iterate over </param>
		''' <param name="rng">           Optional random instance to use for shuffling of order. If null, no shuffling will be used. </param>
		''' <param name="loader">        Loader to use when loading DataSets </param>
		''' <param name="sourceFactory"> The factory to use to convert the paths into streams via <seealso cref="Source"/> </param>
		Public Sub New(ByVal paths As ICollection(Of String), ByVal rng As Random, ByVal loader As Loader(Of MultiDataSet), ByVal sourceFactory As SourceFactory)
			If TypeOf paths Is System.Collections.IList Then
				Me.paths = CType(paths, IList(Of String))
			Else
				Me.paths = New List(Of String)(paths)
			End If
			Me.rng = rng
			Me.loader = loader
			Me.sourceFactory = sourceFactory
			Me.iter = Nothing

			If rng IsNot Nothing Then
				order = New Integer(paths.Count - 1){}
				For i As Integer = 0 To order.Length - 1
					order(i) = i
				Next i
				MathUtils.shuffleArray(order, rng)
			Else
				order = Nothing
			End If
		End Sub

		Public Overridable Function [next](ByVal i As Integer) As MultiDataSet Implements MultiDataSetIterator.next
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return paths IsNot Nothing
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			If Not resetSupported() Then
				 Throw New System.NotSupportedException("Reset not supported when using Iterator<String> instead of Iterable<String>")
			End If
			position = 0
			If rng IsNot Nothing Then
				MathUtils.shuffleArray(order, rng)
			End If
		End Sub

		Public Overrides Function hasNext() As Boolean
			If iter IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return iter.hasNext()
			End If
			Return position < paths.Count
		End Function

		Public Overrides Function [next]() As MultiDataSet
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not hasNext() Then
				Throw New NoSuchElementException("No next element")
			End If
			Dim path As String
			If iter IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				path = iter.next()
			Else
				If order IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: path = paths.get(order[position++]);
					path = paths(order(position))
						position += 1
				Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: path = paths.get(position++);
					path = paths(position)
						position += 1
				End If
			End If
			Dim s As Source = sourceFactory.getSource(path)
			Dim mds As MultiDataSet
			Try
				mds = loader.load(s)
			Catch e As IOException
				Throw New Exception(e)
			End Try
			If preProcessor IsNot Nothing Then
				preProcessor.preProcess(mds)
			End If
			Return mds
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException("Not supported")
		End Sub
	End Class

End Namespace