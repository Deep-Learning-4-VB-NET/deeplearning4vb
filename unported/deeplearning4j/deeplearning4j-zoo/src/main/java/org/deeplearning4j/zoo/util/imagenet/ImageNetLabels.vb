Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports BaseLabels = org.deeplearning4j.zoo.util.BaseLabels
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.deeplearning4j.zoo.util.imagenet


	Public Class ImageNetLabels
		Inherits BaseLabels

		Private Const jsonResource As String = "imagenet_class_index.json"
		Private predictionLabels As List(Of String)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public ImageNetLabels() throws java.io.IOException
		Public Sub New()
			Me.predictionLabels = getLabels()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected java.util.ArrayList<String> getLabels() throws java.io.IOException
		Protected Friend Overrides ReadOnly Property Labels As List(Of String)
			Get
    
				Dim localFile As File = ResourceFile
				If predictionLabels Is Nothing Then
					Dim jsonMap As Dictionary(Of String, List(Of String))
					jsonMap = (New ObjectMapper()).readValue(localFile, GetType(Hashtable))
					predictionLabels = New List(Of String)(jsonMap.Count)
					For i As Integer = 0 To jsonMap.Count - 1
						predictionLabels.Add(jsonMap(i.ToString())(1))
					Next i
				End If
				Return predictionLabels
			End Get
		End Property

		''' <summary>
		''' Returns the description of tne nth class in the 1000 classes of ImageNet. </summary>
		''' <param name="n">
		''' @return </param>
		Public Overrides Function getLabel(ByVal n As Integer) As String
			Return predictionLabels(n)
		End Function

		Protected Friend Overrides ReadOnly Property URL As URL
			Get
				Try
					Return DL4JResources.getURL("resources/imagenet/" & jsonResource)
				Catch e As MalformedURLException
					Throw New Exception(e)
				End Try
			End Get
		End Property

		Protected Friend Overrides Function resourceName() As String
			Return jsonResource
		End Function

		Protected Friend Overrides Function resourceMD5() As String
			Return "c2c37ea517e94d9795004a39431a14cb"
		End Function

		''' <summary>
		''' Given predictions from the trained model this method will return a string
		''' listing the top five matches and the respective probabilities </summary>
		''' <param name="predictions">
		''' @return </param>
		Public Overridable Overloads Function decodePredictions(ByVal predictions As INDArray) As String
			Preconditions.checkState(predictions.size(1) = predictionLabels.Count, "Invalid input array:" & " expected array with size(1) equal to numLabels (%s), got array with shape %s", predictionLabels.Count, predictions.shape())

			Dim predictionDescription As String = ""
			Dim top5(4) As Integer
			Dim top5Prob(4) As Single

			'brute force collect top 5
			Dim i As Integer = 0
			Dim batch As Integer = 0
			Do While batch < predictions.size(0)
				predictionDescription &= "Predictions for batch "
				If predictions.size(0) > 1 Then
					predictionDescription &= batch.ToString()
				End If
				predictionDescription &= " :"
				Dim currentBatch As INDArray = predictions.getRow(batch).dup()
				Do While i < 5
					top5(i) = Nd4j.argMax(currentBatch, 1).getInt(0)
					top5Prob(i) = currentBatch.getFloat(batch, top5(i))
					currentBatch.putScalar(0, top5(i), 0)
					predictionDescription &= vbLf & vbTab & String.Format("{0,3:F}", top5Prob(i) * 100) & "%, " & predictionLabels(top5(i))
					i += 1
				Loop
				batch += 1
			Loop
			Return predictionDescription
		End Function

	End Class

End Namespace