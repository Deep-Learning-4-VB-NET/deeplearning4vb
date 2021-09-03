Imports System
Imports System.Collections.Generic
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

Namespace org.deeplearning4j.datasets.iterator.file


	<Serializable>
	Public Class FileMultiDataSetIterator
		Inherits BaseFileIterator(Of MultiDataSet, MultiDataSetPreProcessor)
		Implements MultiDataSetIterator


		''' <summary>
		''' Create a FileMultiDataSetIterator with the following default settings:<br>
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
		''' Create a FileMultiDataSetIterator with the following default settings:<br>
		''' - Recursive: files in subdirectories are included<br>
		''' - Randomization: order of examples is randomized with a random RNG seed<br>
		''' - Batch size: default (as in the stored DataSets - no splitting/combining)<br>
		''' - File extensions: no filtering - all files in directory are assumed to be a DataSet<br>
		''' </summary>
		''' <param name="rootDirs"> Root directories containing the MultiDataSet objects. MultiDataSets from all of these
		'''                 directories will be included in the iterator output </param>
		Public Sub New(ParamArray ByVal rootDirs() As File)
			Me.New(rootDirs, True, New Random(), -1, DirectCast(Nothing, String()))
		End Sub

		''' <summary>
		''' Create a FileMultiDataSetIterator with the specified batch size, and the following default settings:<br>
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
		''' Create a FileMultiDataSetIterator with filtering based on file extensions, and the following default settings:<br>
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
		''' Create a FileMultiDataSetIterator with the specified batch size, filtering based on file extensions, and the
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
		''' Create a FileMultiDataSetIterator with all settings specified
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
		''' Create a FileMultiDataSetIterator with all settings specified
		''' </summary>
		''' <param name="rootDirs">        Root directories containing the MultiDataSet objects. MultiDataSets from all of these
		'''                        directories will be included in the iterator output </param>
		''' <param name="recursive">       If true: include files in subdirectories </param>
		''' <param name="rng">             May be null. If non-null, use this to randomize order </param>
		''' <param name="batchSize">       Batch size. If > 0, DataSets will be split/recombined as required. If <= 0, DataSets will
		'''                        simply be loaded and returned unmodified </param>
		''' <param name="validExtensions"> May be null. If non-null, only files with one of the specified extensions will be used </param>
		Public Sub New(ByVal rootDirs() As File, ByVal recursive As Boolean, ByVal rng As Random, ByVal batchSize As Integer, ParamArray ByVal validExtensions() As String)
			MyBase.New(rootDirs, recursive, rng, batchSize, validExtensions)
		End Sub

		Protected Friend Overrides Function load(ByVal f As File) As MultiDataSet
			Dim mds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet()
			Try
				mds.load(f)
			Catch e As IOException
				Throw New Exception("Error loading MultiDataSet from file: " & f, e)
			End Try
			Return mds
		End Function

		Protected Friend Overrides Function sizeOf(ByVal [of] As MultiDataSet) As Long
			Return [of].getFeatures(0).size(0)
		End Function

		Protected Friend Overrides Function split(ByVal toSplit As MultiDataSet) As IList(Of MultiDataSet)
			Return toSplit.asList()
		End Function

		Public Overrides Function merge(ByVal toMerge As IList(Of MultiDataSet)) As MultiDataSet
			Return org.nd4j.linalg.dataset.MultiDataSet.merge(toMerge)
		End Function

		Protected Friend Overrides Sub applyPreprocessor(ByVal toPreProcess As MultiDataSet)
			If preProcessor IsNot Nothing Then
				preProcessor.preProcess(toPreProcess)
			End If
		End Sub

		Public Overridable Overloads Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
			Throw New System.NotSupportedException("Not supported for this iterator")
		End Function
	End Class

End Namespace