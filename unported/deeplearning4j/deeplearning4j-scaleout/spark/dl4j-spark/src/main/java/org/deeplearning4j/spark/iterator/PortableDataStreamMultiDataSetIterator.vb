Imports System
Imports System.Collections.Generic
Imports System.IO
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

Namespace org.deeplearning4j.spark.iterator


	<Serializable>
	Public Class PortableDataStreamMultiDataSetIterator
		Implements MultiDataSetIterator

		Private ReadOnly dataSetStreams As ICollection(Of PortableDataStream)
'JAVA TO VB CONVERTER NOTE: The field preprocessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preprocessor_Conflict As MultiDataSetPreProcessor
		Private iter As IEnumerator(Of PortableDataStream)

		Public Sub New(ByVal iter As IEnumerator(Of PortableDataStream))
			Me.dataSetStreams = Nothing
			Me.iter = iter
		End Sub

		Public Sub New(ByVal dataSetStreams As ICollection(Of PortableDataStream))
			Me.dataSetStreams = dataSetStreams
			iter = dataSetStreams.GetEnumerator()
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return [next]()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return dataSetStreams IsNot Nothing
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			If dataSetStreams Is Nothing Then
				Throw New System.InvalidOperationException("Cannot reset iterator constructed with an iterator")
			End If
			iter = dataSetStreams.GetEnumerator()
		End Sub

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
				Me.preprocessor_Conflict = preProcessor
			End Set
			Get
				Return preprocessor_Conflict
			End Get
		End Property


		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iter.hasNext()
		End Function

		Public Overrides Function [next]() As MultiDataSet
			Dim ds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim pds As PortableDataStream = iter.next()
			Try
					Using [is] As Stream = pds.open()
					ds.load([is])
					End Using
			Catch e As IOException
				Throw New Exception("Error loading MultiDataSet at path " & pds.getPath() & " - MultiDataSet may be corrupt or invalid." & " Spark MultiDataSets can be validated using org.deeplearning4j.spark.util.data.SparkDataValidation", e)
			End Try

			If preprocessor_Conflict IsNot Nothing Then
				preprocessor_Conflict.preProcess(ds)
			End If
			Return ds
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub
	End Class

End Namespace