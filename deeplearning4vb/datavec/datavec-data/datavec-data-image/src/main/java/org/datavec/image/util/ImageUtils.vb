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

Namespace org.datavec.image.util

	Public Class ImageUtils

		''' <summary>
		''' Calculate coordinates in an image, assuming the image has been scaled from (oH x oW) pixels to (nH x nW) pixels
		''' </summary>
		''' <param name="x">          X position (pixels) to translate </param>
		''' <param name="y">          Y position (pixels) to translate </param>
		''' <param name="origImageW"> Original image width (pixels) </param>
		''' <param name="origImageH"> Original image height (pixels) </param>
		''' <param name="newImageW">  New image width (pixels) </param>
		''' <param name="newImageH">  New image height (pixels) </param>
		''' <returns>  New X and Y coordinates (pixels, in new image) </returns>
		Public Shared Function translateCoordsScaleImage(ByVal x As Double, ByVal y As Double, ByVal origImageW As Double, ByVal origImageH As Double, ByVal newImageW As Double, ByVal newImageH As Double) As Double()

			Dim newX As Double = x * newImageW / origImageW
			Dim newY As Double = y * newImageH / origImageH

			Return New Double(){newX, newY}
		End Function

	End Class

End Namespace