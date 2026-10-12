Imports System
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports FrameConverter = org.bytedeco.javacv.FrameConverter
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties

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
'ORIGINAL LINE: @NoArgsConstructor @JsonIgnoreProperties({"converter", "currentImage"}) @Data public abstract class BaseImageTransform<F> implements ImageTransform
	Public MustInherit Class BaseImageTransform(Of F)
		Implements ImageTransform

		Protected Friend random As Random
		Protected Friend converter As FrameConverter(Of F)
'JAVA TO VB CONVERTER NOTE: The field currentImage was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend currentImage_Conflict As ImageWritable

		Protected Friend Sub New(ByVal random As Random)
			Me.random = random
		End Sub

		Public Function transform(ByVal image As ImageWritable) As ImageWritable
			Return transform(image, random)
		End Function

		Public Function transform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable Implements ImageTransform.transform
				currentImage_Conflict = doTransform(image, random)
				Return currentImage_Conflict
		End Function

		Protected Friend MustOverride Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable

		Public Overridable Function query(ParamArray ByVal coordinates() As Single) As Single() Implements ImageTransform.query
			Throw New System.NotSupportedException()
		End Function

		Public Overridable ReadOnly Property CurrentImage As ImageWritable Implements ImageTransform.getCurrentImage
			Get
				Return currentImage_Conflict
			End Get
		End Property
	End Class

End Namespace