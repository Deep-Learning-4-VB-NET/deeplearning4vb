Imports System
Imports PathLabelGenerator = org.datavec.api.io.labels.PathLabelGenerator
Imports PathMultiLabelGenerator = org.datavec.api.io.labels.PathMultiLabelGenerator
Imports ImageTransform = org.datavec.image.transform.ImageTransform

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

Namespace org.datavec.image.recordreader


	<Serializable>
	Public Class ImageRecordReader
		Inherits BaseImageRecordReader


		''' <summary>
		''' Loads images with height = 28, width = 28, and channels = 1, appending no labels.
		''' Output format is NCHW (channels first) - [numExamples, 1, 28, 28]
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Loads images with given height, width, and channels, appending labels returned by the generator.
		''' Output format is NCHW (channels first) - [numExamples, channels, height, width]
		''' </summary>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal labelGenerator As PathLabelGenerator)
			MyBase.New(height, width, channels, labelGenerator)
		End Sub

		''' <summary>
		''' Loads images with given height, width, and channels, appending labels returned by the generator.
		''' Output format is NCHW (channels first) - [numExamples, channels, height, width]
		''' </summary>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal labelGenerator As PathMultiLabelGenerator)
			MyBase.New(height, width, channels, labelGenerator)
		End Sub

		''' <summary>
		''' Loads images with given height, width, and channels, appending no labels - in NCHW (channels first) format </summary>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long)
			MyBase.New(height, width, channels, DirectCast(Nothing, PathLabelGenerator))
		End Sub

		''' <summary>
		''' Loads images with given height, width, and channels, appending no labels - in specified format<br>
		''' If {@code nchw_channels_first == true} output format is NCHW (channels first) - [numExamples, channels, height, width]<br>
		''' If {@code nchw_channels_first == false} output format is NHWC (channels last) - [numExamples, height, width, channels]<br>
		''' </summary>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal nchw_channels_first As Boolean)
			MyBase.New(height, width, channels, nchw_channels_first, Nothing, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Loads images with given height, width, and channels, appending labels returned by the generator.
		''' Output format is NCHW (channels first) - [numExamples, channels, height, width] 
		''' </summary>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal labelGenerator As PathLabelGenerator, ByVal imageTransform As ImageTransform)
			MyBase.New(height, width, channels, labelGenerator, imageTransform)
		End Sub

		''' <summary>
		''' Loads images with given height, width, and channels, appending labels returned by the generator.<br>
		''' If {@code nchw_channels_first == true} output format is NCHW (channels first) - [numExamples, channels, height, width]<br>
		''' If {@code nchw_channels_first == false} output format is NHWC (channels last) - [numExamples, height, width, channels]<br>
		''' </summary>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal nchw_channels_first As Boolean, ByVal labelGenerator As PathLabelGenerator, ByVal imageTransform As ImageTransform)
			MyBase.New(height, width, channels, nchw_channels_first, labelGenerator, Nothing, imageTransform)
		End Sub

		''' <summary>
		''' Loads images with given height, width, and channels, appending no labels.
		''' Output format is NCHW (channels first) - [numExamples, channels, height, width]
		''' </summary>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal imageTransform As ImageTransform)
			MyBase.New(height, width, channels, Nothing, imageTransform)
		End Sub

		''' <summary>
		''' Loads images with given  height, width, and channels, appending labels returned by the generator
		''' Output format is NCHW (channels first) - [numExamples, channels, height, width]
		''' </summary>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal labelGenerator As PathLabelGenerator)
			MyBase.New(height, width, 1, labelGenerator)
		End Sub

		''' <summary>
		''' Loads images with given height, width, and channels = 1, appending no labels.
		''' Output format is NCHW (channels first) - [numExamples, channels, height, width]
		''' </summary>
		Public Sub New(ByVal height As Long, ByVal width As Long)
			MyBase.New(height, width, 1, Nothing, Nothing)
		End Sub
	End Class

End Namespace