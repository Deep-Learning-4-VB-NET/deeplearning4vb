Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.nn.layers.objdetect

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class DetectedObject
	Public Class DetectedObject

		Private ReadOnly exampleNumber As Integer
		Private ReadOnly centerX As Double
		Private ReadOnly centerY As Double
		Private ReadOnly width As Double
		Private ReadOnly height As Double
		Private ReadOnly classPredictions As INDArray
'JAVA TO VB CONVERTER NOTE: The field predictedClass was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private predictedClass_Conflict As Integer = -1
		Private ReadOnly confidence As Double


		''' <param name="exampleNumber">    Index of the example in the current minibatch. For single images, this is always 0 </param>
		''' <param name="centerX">          Center X position of the detected object </param>
		''' <param name="centerY">          Center Y position of the detected object </param>
		''' <param name="width">            Width of the detected object </param>
		''' <param name="height">           Height of  the detected object </param>
		''' <param name="classPredictions"> Row vector of class probabilities for the detected object </param>
		Public Sub New(ByVal exampleNumber As Integer, ByVal centerX As Double, ByVal centerY As Double, ByVal width As Double, ByVal height As Double, ByVal classPredictions As INDArray, ByVal confidence As Double)
			Me.exampleNumber = exampleNumber
			Me.centerX = centerX
			Me.centerY = centerY
			Me.width = width
			Me.height = height
			Me.classPredictions = classPredictions
			Me.confidence = confidence
		End Sub

		''' <summary>
		''' Get the top left X/Y coordinates of the detected object
		''' </summary>
		''' <returns> Array of length 2 - top left X and Y </returns>
		Public Overridable ReadOnly Property TopLeftXY As Double()
			Get
				Return New Double(){ centerX - width / 2.0, centerY - height / 2.0}
			End Get
		End Property

		''' <summary>
		''' Get the bottom right X/Y coordinates of the detected object
		''' </summary>
		''' <returns> Array of length 2 - bottom right X and Y </returns>
		Public Overridable ReadOnly Property BottomRightXY As Double()
			Get
				Return New Double(){ centerX + width / 2.0, centerY + height / 2.0}
			End Get
		End Property

		''' <summary>
		''' Get the index of the predicted class (based on maximum predicted probability) </summary>
		''' <returns> Index of the predicted class (0 to nClasses - 1) </returns>
		Public Overridable ReadOnly Property PredictedClass As Integer
			Get
				If predictedClass_Conflict = -1 Then
					If classPredictions.rank() = 1 Then
						predictedClass_Conflict = classPredictions.argMax().getInt(0)
					Else
						' ravel in case we get a column vector, or rank 2 row vector, etc
						predictedClass_Conflict = classPredictions.ravel().argMax().getInt(0)
					End If
				End If
				Return predictedClass_Conflict
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return "DetectedObject(exampleNumber=" & exampleNumber & ", centerX=" & centerX & ", centerY=" & centerY & ", width=" & width & ", height=" & height & ", confidence=" & confidence & ", classPredictions=" & classPredictions & ", predictedClass=" & PredictedClass & ")"
		End Function
	End Class

End Namespace