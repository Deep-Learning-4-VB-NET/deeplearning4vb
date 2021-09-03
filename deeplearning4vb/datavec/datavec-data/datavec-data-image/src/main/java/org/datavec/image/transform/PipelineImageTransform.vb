Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports org.bytedeco.opencv.opencv_core

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

Namespace org.datavec.image.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class PipelineImageTransform extends BaseImageTransform<Mat>
	Public Class PipelineImageTransform
		Inherits BaseImageTransform(Of Mat)

		Protected Friend imageTransforms As IList(Of Pair(Of ImageTransform, Double))
		Protected Friend shuffle As Boolean
		Protected Friend rng As org.nd4j.linalg.api.rng.Random

		Protected Friend currentTransforms As IList(Of ImageTransform) = New List(Of ImageTransform)()

		Public Sub New(ParamArray ByVal transforms() As ImageTransform)
			Me.New(1234, False, transforms)
		End Sub

		Public Sub New(ByVal seed As Long, ByVal shuffle As Boolean, ParamArray ByVal transforms() As ImageTransform)
			MyBase.New(Nothing) ' for perf reasons we ignore java Random, create our own

			Dim pipeline As IList(Of Pair(Of ImageTransform, Double)) = New LinkedList(Of Pair(Of ImageTransform, Double))()
			For i As Integer = 0 To transforms.Length - 1
				pipeline.Add(New Pair(Of ImageTransform, Double)(transforms(i), 1.0))
			Next i

			Me.imageTransforms = pipeline
			Me.shuffle = shuffle
			Me.rng = Nd4j.Random
			rng.Seed = seed
		End Sub

		Public Sub New(ByVal transforms As IList(Of Pair(Of ImageTransform, Double)))
			Me.New(1234, transforms, False)
		End Sub

		Public Sub New(ByVal transforms As IList(Of Pair(Of ImageTransform, Double)), ByVal shuffle As Boolean)
			Me.New(1234, transforms, shuffle)
		End Sub

		Public Sub New(ByVal seed As Long, ByVal transforms As IList(Of Pair(Of ImageTransform, Double)))
			Me.New(seed, transforms, False)
		End Sub

		Public Sub New(ByVal seed As Long, ByVal transforms As IList(Of Pair(Of ImageTransform, Double)), ByVal shuffle As Boolean)
			Me.New(Nothing, seed, transforms, shuffle)
		End Sub

		Public Sub New(ByVal random As Random, ByVal seed As Long, ByVal transforms As IList(Of Pair(Of ImageTransform, Double)), ByVal shuffle As Boolean)
			MyBase.New(random) ' used by the transforms in the pipeline
			Me.imageTransforms = transforms
			Me.shuffle = shuffle
			Me.rng = Nd4j.Random
			rng.Seed = seed
		End Sub

		''' <summary>
		''' Takes an image and executes a pipeline of combined transforms.
		''' </summary>
		''' <param name="image"> to transform, null == end of stream </param>
		''' <param name="random"> object to use (or null for deterministic) </param>
		''' <returns> transformed image </returns>
		Protected Friend Overrides Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable
			If shuffle Then
				Collections.shuffle(imageTransforms)
			End If

			currentTransforms.Clear()

			' execute each item in the pipeline
			For Each tuple As Pair(Of ImageTransform, Double) In imageTransforms
				If tuple.Second = 1.0 OrElse rng.nextDouble() < tuple.Second Then ' probability of execution
					currentTransforms.Add(tuple.First)
					image = If(random IsNot Nothing, tuple.First.transform(image, random), tuple.First.transform(image))
				End If
			Next tuple

			Return image
		End Function

		Public Overrides Function query(ParamArray ByVal coordinates() As Single) As Single()
			For Each transform As ImageTransform In currentTransforms
				coordinates = transform.query(coordinates)
			Next transform
			Return coordinates
		End Function

		''' <summary>
		''' Optional builder helper for PipelineImageTransform
		''' </summary>
		Public Class Builder

			Protected Friend imageTransforms As IList(Of Pair(Of ImageTransform, Double)) = New List(Of Pair(Of ImageTransform, Double))()
'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend seed_Conflict As Long? = Nothing

			''' <summary>
			''' This method sets RNG seet for this pipeline
			''' </summary>
			''' <param name="seed">
			''' @return </param>
			Public Overridable Function setSeed(ByVal seed As Long) As Builder
				Me.seed_Conflict = seed
				Return Me
			End Function

			''' <summary>
			''' This method adds given transform with 100% invocation probability to this pipelien
			''' </summary>
			''' <param name="transform">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addImageTransform(@NonNull ImageTransform transform)
			Public Overridable Function addImageTransform(ByVal transform As ImageTransform) As Builder
				Return addImageTransform(transform, 1.0)
			End Function

			''' <summary>
			''' This method adds given transform with given invocation probability to this pipelien
			''' </summary>
			''' <param name="transform"> </param>
			''' <param name="probability">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addImageTransform(@NonNull ImageTransform transform, System.Nullable<Double> probability)
			Public Overridable Function addImageTransform(ByVal transform As ImageTransform, ByVal probability As Double?) As Builder
				If probability < 0.0 Then
					probability = 0.0
				End If
				If probability > 1.0 Then
					probability = 1.0
				End If

				imageTransforms.Add(Pair.makePair(transform, probability))
				Return Me
			End Function

			''' <summary>
			''' This method returns new PipelineImageTransform instance
			''' 
			''' @return
			''' </summary>
			Public Overridable Function build() As PipelineImageTransform
				If seed_Conflict IsNot Nothing Then
					Return New PipelineImageTransform(seed_Conflict, imageTransforms)
				Else
					Return New PipelineImageTransform(imageTransforms)
				End If
			End Function
		End Class
	End Class

End Namespace