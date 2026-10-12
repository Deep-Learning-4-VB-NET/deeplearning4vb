Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Mode = org.nd4j.linalg.api.ops.impl.transforms.Pad.Mode
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.nd4j.linalg.convolution

	Public Class OldConvolution

		Private Sub New()
		End Sub

		''' 
		''' <param name="col"> </param>
		''' <param name="stride"> </param>
		''' <param name="padding"> </param>
		''' <param name="height"> </param>
		''' <param name="width">
		''' @return </param>
		Public Shared Function col2im(ByVal col As INDArray, ByVal stride() As Integer, ByVal padding() As Integer, ByVal height As Integer, ByVal width As Integer) As INDArray
			Return col2im(col, stride(0), stride(1), padding(0), padding(1), height, width)
		End Function

		''' <summary>
		''' Rearrange matrix
		''' columns into blocks
		''' </summary>
		''' <param name="col"> the column
		'''            transposed image to convert </param>
		''' <param name="sy"> stride y </param>
		''' <param name="sx"> stride x </param>
		''' <param name="ph"> padding height </param>
		''' <param name="pw"> padding width </param>
		''' <param name="h"> height </param>
		''' <param name="w"> width
		''' @return </param>
		Public Shared Function col2im(ByVal col As INDArray, ByVal sy As Integer, ByVal sx As Integer, ByVal ph As Integer, ByVal pw As Integer, ByVal h As Integer, ByVal w As Integer) As INDArray
			'number of images
			Dim n As Long = col.size(0)
			'number of columns
			Dim c As Long = col.size(1)
			'kernel height
			Dim kh As Long = col.size(2)
			'kernel width
			Dim kw As Long = col.size(3)
			'out height
			Dim outH As Long = col.size(4)
			'out width
			Dim outW As Long = col.size(5)

			Dim img As INDArray = Nd4j.create(n, c, h + 2 * ph + sy - 1, w + 2 * pw + sx - 1)
			For i As Integer = 0 To kh - 1
				'iterate over the kernel rows
				Dim iLim As Long = i + sy * outH
				For j As Integer = 0 To kw - 1
					'iterate over the kernel columns
					Dim jLim As Long = j + sx * outW
					Dim indices() As INDArrayIndex = {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(i, sy, iLim), NDArrayIndex.interval(j, sx, jLim)}

					Dim get As INDArray = img.get(indices)

					Dim colAdd As INDArray = col.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i), NDArrayIndex.point(j), NDArrayIndex.all(), NDArrayIndex.all())
					get.addi(colAdd)
					img.put(indices, get)

				Next j
			Next i

			'return the subset of the padded image relative to the height/width of the image and the padding width/height
			Return img.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(ph, ph + h), NDArrayIndex.interval(pw, pw + w))
		End Function

		''' 
		''' <param name="img"> </param>
		''' <param name="kernel"> </param>
		''' <param name="stride"> </param>
		''' <param name="padding">
		''' @return </param>
		Public Shared Function im2col(ByVal img As INDArray, ByVal kernel() As Integer, ByVal stride() As Integer, ByVal padding() As Integer) As INDArray
			Return im2col(img, kernel(0), kernel(1), stride(0), stride(1), padding(0), padding(1), 0, False)
		End Function

		''' <summary>
		''' Implement column formatted images </summary>
		''' <param name="img"> the image to process </param>
		''' <param name="kh"> the kernel height </param>
		''' <param name="kw"> the kernel width </param>
		''' <param name="sy"> the stride along y </param>
		''' <param name="sx"> the stride along x </param>
		''' <param name="ph"> the padding width </param>
		''' <param name="pw"> the padding height </param>
		''' <param name="pval"> the padding value </param>
		''' <param name="coverAll"> whether to cover the whole image or not </param>
		''' <returns> the column formatted image
		'''  </returns>
		Public Shared Function im2col(ByVal img As INDArray, ByVal kh As Integer, ByVal kw As Integer, ByVal sy As Integer, ByVal sx As Integer, ByVal ph As Integer, ByVal pw As Integer, ByVal pval As Integer, ByVal coverAll As Boolean) As INDArray
			'number of images
			Dim n As Long = img.size(0)
			'number of channels (depth)
			Dim c As Long = img.size(1)
			'image height
			Dim h As Long = img.size(2)
			'image width
			Dim w As Long = img.size(3)
			Dim outHeight As Long = outSize(h, kh, sy, ph, coverAll)
			Dim outWidth As Long = outSize(w, kw, sx, pw, coverAll)
			Dim padded As INDArray = Nd4j.pad(img, New Integer()() {
				New Integer() {0, 0},
				New Integer() {0, 0},
				New Integer() {ph, ph + sy - 1},
				New Integer() {pw, pw + sx - 1}
			}, Mode.CONSTANT, pval)
			Dim ret As INDArray = Nd4j.create(n, c, kh, kw, outHeight, outWidth)
			For i As Integer = 0 To kh - 1
				'offset for the row based on the stride and output height
				Dim iLim As Long = i + sy * outHeight
				For j As Integer = 0 To kw - 1
					'offset for the column based on stride and output width
					Dim jLim As Long = j + sx * outWidth
					Dim get As INDArray = padded.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(i, sy, iLim), NDArrayIndex.interval(j, sx, jLim))
					ret.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i), NDArrayIndex.point(j), NDArrayIndex.all(), NDArrayIndex.all()}, get)
				Next j
			Next i
			Return ret
		End Function

		''' 
		''' <summary>
		''' The out size for a convolution </summary>
		''' <param name="size"> </param>
		''' <param name="k"> </param>
		''' <param name="s"> </param>
		''' <param name="p"> </param>
		''' <param name="coverAll">
		''' @return </param>
		Public Shared Function outSize(ByVal size As Integer, ByVal k As Integer, ByVal s As Integer, ByVal p As Integer, ByVal coverAll As Boolean) As Integer
			If coverAll Then
				Return (size + p * 2 - k + s - 1) \ s + 1
			Else
				Return (size + p * 2 - k) \ s + 1
			End If
		End Function

		Public Shared Function outSize(ByVal size As Long, ByVal k As Long, ByVal s As Long, ByVal p As Long, ByVal coverAll As Boolean) As Long
			If coverAll Then
				Return (size + p * 2 - k + s - 1) \ s + 1
			Else
				Return (size + p * 2 - k) \ s + 1
			End If
		End Function

	End Class

End Namespace