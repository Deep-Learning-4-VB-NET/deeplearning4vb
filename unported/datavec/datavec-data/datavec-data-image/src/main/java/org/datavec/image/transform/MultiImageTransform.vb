Imports System
Imports Data = lombok.Data
Imports ImageWritable = org.datavec.image.data.ImageWritable
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
'ORIGINAL LINE: @Data public class MultiImageTransform extends BaseImageTransform<Mat>
	Public Class MultiImageTransform
		Inherits BaseImageTransform(Of Mat)

		Private transform As PipelineImageTransform

		Public Sub New(ParamArray ByVal transforms() As ImageTransform)
			Me.New(Nothing, transforms)
		End Sub

		Public Sub New(ByVal random As Random, ParamArray ByVal transforms() As ImageTransform)
			MyBase.New(random)
			transform = New PipelineImageTransform(transforms)
		End Sub

		Protected Friend Overrides Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable
			Return If(random Is Nothing, transform.transform(image), transform.transform(image, random))
		End Function

		Public Overrides Function query(ParamArray ByVal coordinates() As Single) As Single()
			Return transform.query(coordinates)
		End Function
	End Class

End Namespace