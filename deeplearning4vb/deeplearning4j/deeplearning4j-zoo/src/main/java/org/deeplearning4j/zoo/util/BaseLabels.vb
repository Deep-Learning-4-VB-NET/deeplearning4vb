Imports System
Imports System.Collections.Generic
Imports System.IO
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports ResourceType = org.deeplearning4j.common.resources.ResourceType
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Downloader = org.nd4j.common.resources.Downloader

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

Namespace org.deeplearning4j.zoo.util


	Public MustInherit Class BaseLabels
		Implements Labels

		Protected Friend labels As List(Of String)

		''' <summary>
		''' Override <seealso cref="getLabels()"/> when using this constructor. </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected BaseLabels() throws IOException
		Protected Friend Sub New()
			Me.labels = getLabels()
		End Sub

		''' <summary>
		''' No need to override anything with this constructor.
		''' </summary>
		''' <param name="textResource"> name of a resource containing labels as a list in a text file. </param>
		''' <exception cref="IOException">  </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected BaseLabels(String textResource) throws IOException
		Protected Friend Sub New(ByVal textResource As String)
			Me.labels = getLabels(textResource)
		End Sub

		''' <summary>
		''' Override to return labels when not calling <seealso cref="BaseLabels(String)"/>.
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected java.util.ArrayList<String> getLabels() throws IOException
		Protected Friend Overridable ReadOnly Property Labels As List(Of String)
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns labels based on the text file resource.
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected java.util.ArrayList<String> getLabels(String textResource) throws IOException
		Protected Friend Overridable Function getLabels(ByVal textResource As String) As List(Of String)
			Dim labels As New List(Of String)()
			Dim resourceFile As File = Me.ResourceFile 'Download if required
			Using [is] As Stream = New BufferedInputStream(New FileStream(resourceFile, FileMode.Open, FileAccess.Read)), s As New java.util.Scanner([is])
				Do While s.hasNextLine()
					labels.Add(s.nextLine())
				Loop
			End Using
			Return labels
		End Function

		Public Overridable Function getLabel(ByVal n As Integer) As String Implements Labels.getLabel
			Preconditions.checkArgument(n >= 0 AndAlso n < labels.Count, "Invalid index: %s. Must be in range" & "0 <= n < %s", n, labels.Count)
			Return labels(n)
		End Function

		Public Overridable Function decodePredictions(ByVal predictions As INDArray, ByVal n As Integer) As IList(Of IList(Of ClassPrediction)) Implements Labels.decodePredictions
			If predictions.rank() = 1 Then
				'Reshape 1d edge case to [1, nClasses] 2d
				predictions = predictions.reshape(ChrW(1), predictions.length())
			End If
			Preconditions.checkState(predictions.size(1) = labels.Count, "Invalid input array:" & " expected array with size(1) equal to numLabels (%s), got array with shape %s", labels.Count, predictions.shape())

			Dim rows As Long = predictions.size(0)
			Dim cols As Long = predictions.size(1)
			If predictions.ColumnVectorOrScalar Then
				predictions = predictions.ravel()
				rows = CInt(predictions.size(0))
				cols = CInt(predictions.size(1))
			End If
			Dim descriptions As IList(Of IList(Of ClassPrediction)) = New List(Of IList(Of ClassPrediction))()
			For batch As Integer = 0 To rows - 1
				Dim result As INDArray = predictions.getRow(batch, True)
				result = Nd4j.vstack(Nd4j.linspace(result.dataType(), 0, cols, 1).reshape(ChrW(1), cols), result)
				result = Nd4j.sortColumns(result, 1, False)
				Dim current As IList(Of ClassPrediction) = New List(Of ClassPrediction)()
				For i As Integer = 0 To n - 1
					Dim label As Integer = result.getInt(0, i)
					Dim prob As Double = result.getDouble(1, i)
					current.Add(New ClassPrediction(label, getLabel(label), prob))
				Next i
				descriptions.Add(current)
			Next batch
			Return descriptions
		End Function

		''' <returns> URL of the resource to download </returns>
		Protected Friend MustOverride ReadOnly Property URL As URL

		''' <returns> Name of the resource (used for inferring local storage parent directory) </returns>
		Protected Friend MustOverride Function resourceName() As String

		''' <returns> MD5 of the resource at getURL() </returns>
		Protected Friend MustOverride Function resourceMD5() As String

		''' <summary>
		''' Download the resource at getURL() to the local resource directory, and return the local copy as a File
		''' </summary>
		''' <returns> File of the local resource </returns>
		Protected Friend Overridable ReadOnly Property ResourceFile As File
			Get
    
				Dim url As URL = Me.URL
				Dim urlString As String = url.ToString()
				Dim filename As String = urlString.Substring(urlString.LastIndexOf("/"c)+1)
				Dim resourceDir As File = DL4JResources.getDirectory(ResourceType.RESOURCE, resourceName())
				Dim localFile As New File(resourceDir, filename)
    
				Dim expMD5 As String = resourceMD5()
				If localFile.exists() Then
					Try
						If Downloader.checkMD5OfFile(expMD5, localFile) Then
							Return localFile
						End If
					Catch e As IOException
						'Ignore
					End Try
					'MD5 failed
					localFile.delete()
				End If
    
				'Download
				Try
					Downloader.download(resourceName(), url, localFile, expMD5, 3)
				Catch e As IOException
					Throw New Exception("Error downloading labels",e)
				End Try
    
				Return localFile
			End Get
		End Property

	End Class

End Namespace