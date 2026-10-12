Imports System
Imports System.Threading
Imports Data = lombok.Data
Imports CanvasFrame = org.bytedeco.javacv.CanvasFrame
Imports Frame = org.bytedeco.javacv.Frame
Imports ImageWritable = org.datavec.image.data.ImageWritable

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
'ORIGINAL LINE: @Data public class ShowImageTransform extends BaseImageTransform
	Public Class ShowImageTransform
		Inherits BaseImageTransform

		Friend canvas As CanvasFrame
		Friend title As String
		Friend delay As Integer

		''' <summary>
		''' Calls {@code this(canvas, -1)}. </summary>
		Public Sub New(ByVal canvas As CanvasFrame)
			Me.New(canvas, -1)
		End Sub

		''' <summary>
		''' Constructs an instance of the ImageTransform from a <seealso cref="CanvasFrame"/>.
		''' </summary>
		''' <param name="canvas"> to display images in </param>
		''' <param name="delay">  max time to wait in milliseconds (0 == infinity, negative == no wait) </param>
		Public Sub New(ByVal canvas As CanvasFrame, ByVal delay As Integer)
			MyBase.New(Nothing)
			Me.canvas = canvas
			Me.delay = delay
		End Sub

		''' <summary>
		''' Calls {@code this(title, -1)}. </summary>
		Public Sub New(ByVal title As String)
			Me.New(title, -1)
		End Sub

		''' <summary>
		''' Constructs an instance of the ImageTransform with a new <seealso cref="CanvasFrame"/>.
		''' </summary>
		''' <param name="title"> of the new CanvasFrame to display images in </param>
		''' <param name="delay"> max time to wait in milliseconds (0 == infinity, negative == no wait) </param>
		Public Sub New(ByVal title As String, ByVal delay As Integer)
			MyBase.New(Nothing)
			Me.canvas = Nothing
			Me.title = title
			Me.delay = delay
		End Sub

		Protected Friend Overrides Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable
			If canvas Is Nothing Then
				canvas = New CanvasFrame(title, 1.0)
				canvas.setDefaultCloseOperation(JFrame.DISPOSE_ON_CLOSE)
			End If
			If image Is Nothing Then
				canvas.dispose()
				Return Nothing
			End If
			If Not canvas.isVisible() Then
				Return image
			End If
			Dim frame As Frame = image.Frame
			canvas.setCanvasSize(frame.imageWidth, frame.imageHeight)
			canvas.showImage(frame)
			If delay >= 0 Then
				Try
					canvas.waitKey(delay)
				Catch ex As InterruptedException
					' reset interrupt to be nice
					Thread.CurrentThread.Interrupt()
				End Try
			End If
			Return image
		End Function

		Public Overrides Function query(ParamArray ByVal coordinates() As Single) As Single()
			Return coordinates
		End Function
	End Class

End Namespace