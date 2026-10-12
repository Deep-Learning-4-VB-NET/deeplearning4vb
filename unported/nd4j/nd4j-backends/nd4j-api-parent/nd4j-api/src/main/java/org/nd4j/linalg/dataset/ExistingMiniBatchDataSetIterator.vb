Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator

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

Namespace org.nd4j.linalg.dataset


	<Serializable>
	Public Class ExistingMiniBatchDataSetIterator
		Implements DataSetIterator

		Public Const DEFAULT_PATTERN As String = "dataset-%d.bin"

		Private currIdx As Integer
		Private rootDir As File
		Private totalBatches As Integer = -1
		Private dataSetPreProcessor As DataSetPreProcessor
		Private ReadOnly pattern As String

		''' <summary>
		''' Create with the given root directory, using the default filename pattern <seealso cref="DEFAULT_PATTERN"/> </summary>
		''' <param name="rootDir"> the root directory to use </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExistingMiniBatchDataSetIterator(@NonNull File rootDir)
		Public Sub New(ByVal rootDir As File)
			Me.New(rootDir, DEFAULT_PATTERN)
		End Sub

		''' 
		''' <param name="rootDir">    The root directory to use </param>
		''' <param name="pattern">    The filename pattern to use. Used with {@code String.format(pattern,idx)}, where idx is an
		'''                   integer, starting at 0. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExistingMiniBatchDataSetIterator(@NonNull File rootDir, String pattern)
		Public Sub New(ByVal rootDir As File, ByVal pattern As String)
			Me.rootDir = rootDir
			totalBatches = rootDir.list().length
			Me.pattern = pattern
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As DataSet
			Throw New System.NotSupportedException("Unable to load custom number of examples")
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			currIdx = 0
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.dataSetPreProcessor = preProcessor
			End Set
			Get
				Return dataSetPreProcessor
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			Return currIdx < totalBatches
		End Function

		Public Overrides Sub remove()
			'no opt;
		End Sub

		Public Overrides Function [next]() As DataSet
			Try
				Dim ret As DataSet = read(currIdx)
				If dataSetPreProcessor IsNot Nothing Then
					dataSetPreProcessor.preProcess(ret)
				End If
				currIdx += 1

				Return ret
			Catch e As IOException
				Throw New System.InvalidOperationException("Unable to read dataset")
			End Try
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private DataSet read(int idx) throws java.io.IOException
		Private Function read(ByVal idx As Integer) As DataSet
			Dim path As New File(rootDir, String.format(pattern, idx))
			Dim d As New DataSet()
			d.load(path)
			Return d
		End Function
	End Class

End Namespace