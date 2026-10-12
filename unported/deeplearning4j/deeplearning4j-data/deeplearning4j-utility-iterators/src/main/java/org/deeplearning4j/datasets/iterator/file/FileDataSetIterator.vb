Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports DataSet = org.nd4j.linalg.dataset.DataSet
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

Namespace org.deeplearning4j.datasets.iterator.file


	<Serializable>
	Public Class FileDataSetIterator
		Inherits BaseFileIterator(Of DataSet, DataSetPreProcessor)
		Implements DataSetIterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private java.util.List<String> labels;
		Private labels As IList(Of String)

		''' <summary>
		''' Create a FileDataSetIterator with the following default settings:<br>
		''' - Recursive: files in subdirectories are included<br>
		''' - Randomization: order of examples is randomized with a random RNG seed<br>
		''' - Batch size: default (as in the stored DataSets - no splitting/combining)<br>
		''' - File extensions: no filtering - all files in directory are assumed to be a DataSet<br>
		''' </summary>
		''' <param name="rootDir"> Root directory containing the DataSet objects </param>
		Public Sub New(ByVal rootDir As File)
			Me.New(rootDir, True, New Random(), -1, DirectCast(Nothing, String()))
		End Sub

		''' <summary>
		''' Create a FileDataSetIterator with the following default settings:<br>
		''' - Recursive: files in subdirectories are included<br>
		''' - Randomization: order of examples is randomized with a random RNG seed<br>
		''' - Batch size: default (as in the stored DataSets - no splitting/combining)<br>
		''' - File extensions: no filtering - all files in directory are assumed to be a DataSet<br>
		''' </summary>
		''' <param name="rootDirs"> Root directories containing the DataSet objects. DataSets from all of these directories will
		'''                 be included in the iterator output </param>
		Public Sub New(ParamArray ByVal rootDirs() As File)
			Me.New(rootDirs, True, New Random(), -1, DirectCast(Nothing, String()))
		End Sub

		''' <summary>
		''' Create a FileDataSetIterator with the specified batch size, and the following default settings:<br>
		''' - Recursive: files in subdirectories are included<br>
		''' - Randomization: order of examples is randomized with a random RNG seed<br>
		''' - File extensions: no filtering - all files in directory are assumed to be a DataSet<br>
		''' </summary>
		''' <param name="rootDir">   Root directory containing the saved DataSet objects </param>
		''' <param name="batchSize"> Batch size. If > 0, DataSets will be split/recombined as required. If <= 0, DataSets will
		'''                  simply be loaded and returned unmodified </param>
		Public Sub New(ByVal rootDir As File, ByVal batchSize As Integer)
			Me.New(rootDir, batchSize, DirectCast(Nothing, String()))
		End Sub

		''' <summary>
		''' Create a FileDataSetIterator with filtering based on file extensions, and the following default settings:<br>
		''' - Recursive: files in subdirectories are included<br>
		''' - Randomization: order of examples is randomized with a random RNG seed<br>
		''' - Batch size: default (as in the stored DataSets - no splitting/combining)<br>
		''' </summary>
		''' <param name="rootDir">         Root directory containing the saved DataSet objects </param>
		''' <param name="validExtensions"> May be null. If non-null, only files with one of the specified extensions will be used </param>
		Public Sub New(ByVal rootDir As File, ParamArray ByVal validExtensions() As String)
			MyBase.New(rootDir, -1, validExtensions)
		End Sub

		''' <summary>
		''' Create a FileDataSetIterator with the specified batch size, filtering based on file extensions, and the
		''' following default settings:<br>
		''' - Recursive: files in subdirectories are included<br>
		''' - Randomization: order of examples is randomized with a random RNG seed<br>
		''' </summary>
		''' <param name="rootDir">         Root directory containing the saved DataSet objects </param>
		''' <param name="batchSize">       Batch size. If > 0, DataSets will be split/recombined as required. If <= 0, DataSets will
		'''                        simply be loaded and returned unmodified </param>
		''' <param name="validExtensions"> May be null. If non-null, only files with one of the specified extensions will be used </param>
		Public Sub New(ByVal rootDir As File, ByVal batchSize As Integer, ParamArray ByVal validExtensions() As String)
			MyBase.New(rootDir, batchSize, validExtensions)
		End Sub

		''' <summary>
		''' Create a FileDataSetIterator with all settings specified
		''' </summary>
		''' <param name="rootDir">         Root directory containing the saved DataSet objects </param>
		''' <param name="recursive">       If true: include files in subdirectories </param>
		''' <param name="rng">             May be null. If non-null, use this to randomize order </param>
		''' <param name="batchSize">       Batch size. If > 0, DataSets will be split/recombined as required. If <= 0, DataSets will
		'''                        simply be loaded and returned unmodified </param>
		''' <param name="validExtensions"> May be null. If non-null, only files with one of the specified extensions will be used </param>
		Public Sub New(ByVal rootDir As File, ByVal recursive As Boolean, ByVal rng As Random, ByVal batchSize As Integer, ParamArray ByVal validExtensions() As String)
			Me.New(New File(){rootDir}, recursive, rng, batchSize, validExtensions)
		End Sub

		''' <summary>
		''' Create a FileDataSetIterator with all settings specified
		''' </summary>
		''' <param name="rootDirs">        Root directories containing the DataSet objects. DataSets from all of these directories will
		'''                        be included in the iterator output </param>
		''' <param name="recursive">       If true: include files in subdirectories </param>
		''' <param name="rng">             May be null. If non-null, use this to randomize order </param>
		''' <param name="batchSize">       Batch size. If > 0, DataSets will be split/recombined as required. If <= 0, DataSets will
		'''                        simply be loaded and returned unmodified </param>
		''' <param name="validExtensions"> May be null. If non-null, only files with one of the specified extensions will be used </param>
		Public Sub New(ByVal rootDirs() As File, ByVal recursive As Boolean, ByVal rng As Random, ByVal batchSize As Integer, ParamArray ByVal validExtensions() As String)
			MyBase.New(rootDirs, recursive, rng, batchSize, validExtensions)
		End Sub

		Protected Friend Overrides Function load(ByVal f As File) As DataSet
			Dim ds As New DataSet()
			ds.load(f)
			Return ds
		End Function

		Protected Friend Overrides Function sizeOf(ByVal [of] As DataSet) As Long
			Return [of].numExamples()
		End Function

		Protected Friend Overrides Function split(ByVal toSplit As DataSet) As IList(Of DataSet)
			Return toSplit.asList()
		End Function

		Protected Friend Overrides Function merge(ByVal toMerge As IList(Of DataSet)) As DataSet
			Return DataSet.merge(toMerge)
		End Function

		Protected Friend Overrides Sub applyPreprocessor(ByVal toPreProcess As DataSet)
			If preProcessor IsNot Nothing Then
				preProcessor.preProcess(toPreProcess)
			End If
		End Sub

		Public Overridable Overloads Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Throw New System.NotSupportedException("Not supported for this iterator")
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Throw New System.NotSupportedException("Not supported for this iterator")
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Throw New System.NotSupportedException("Not supported for this iterator")
		End Function

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return batchSize
		End Function
	End Class

End Namespace