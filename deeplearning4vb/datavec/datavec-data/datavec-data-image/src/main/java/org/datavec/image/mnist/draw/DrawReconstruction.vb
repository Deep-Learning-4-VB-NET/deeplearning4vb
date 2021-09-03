Imports System
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms

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

Namespace org.datavec.image.mnist.draw



	Public Class DrawReconstruction

		Public frame As JFrame
		Friend img As BufferedImage
		Private data As INDArray
		Private width As Integer = 28
		Private height As Integer = 28
		Public title As String = "TEST"
		Private heightOffset As Integer = 0
		Private widthOffset As Integer = 0


		Public Sub New(ByVal data As INDArray, ByVal heightOffset As Integer, ByVal widthOffset As Integer)
			img = New BufferedImage(width, height, BufferedImage.TYPE_INT_RGB)
			Me.data = data
			Me.heightOffset = heightOffset
			Me.widthOffset = widthOffset


		End Sub

		Public Sub New(ByVal data As INDArray)
			img = New BufferedImage(width, height, BufferedImage.TYPE_INT_RGB)
			Me.data = Transforms.round(data)


		End Sub

		Public Overridable Sub readjustToData()
			Me.width = data.columns()
			Me.height = data.rows()
			img = New BufferedImage(width, height, BufferedImage.TYPE_INT_RGB)

		End Sub


		Public Overridable Sub draw()
			Dim r As WritableRaster = img.getRaster()
			Dim equiv(CInt(data.length()) - 1) As Integer
			Dim dataLinear As INDArray = data.reshape(ChrW(-1))
			For i As Integer = 0 To equiv.Length - 1
				equiv(i) = CLng(Math.Round(dataLinear.getInt(i), MidpointRounding.AwayFromZero))
			Next i

			r.setDataElements(0, 0, width, height, equiv)



			frame = New JFrame(title)
			frame.setVisible(True)
			start()
			frame.add(New JLabel(New ImageIcon(Image)))

			frame.pack()
			' Better to DISPOSE than EXIT
			frame.setDefaultCloseOperation(JFrame.DISPOSE_ON_CLOSE)
		End Sub

		Public Overridable Sub close()
			frame.dispose()
		End Sub

		Public Overridable ReadOnly Property Image As Image
			Get
				Return img
			End Get
		End Property

		Public Overridable Sub start()


			Dim pixels() As Integer = CType(img.getRaster().getDataBuffer(), DataBufferInt).getData()
			Dim running As Boolean = True
			Do While running
				Dim bs As BufferStrategy = frame.getBufferStrategy()
				If bs Is Nothing Then
					frame.createBufferStrategy(4)
					Return
				End If
				Dim i As Integer = 0
				Do While i < width * height
					pixels(i) = 0
					i += 1
				Loop

				Dim g As Graphics = bs.getDrawGraphics()
				g.drawImage(img, heightOffset, widthOffset, width, height, Nothing)
				g.dispose()
				bs.show()

			Loop
		End Sub
	End Class

End Namespace