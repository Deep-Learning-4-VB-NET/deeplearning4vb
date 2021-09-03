Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Broadcast = org.nd4j.linalg.factory.Broadcast
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports org.nd4j.linalg.indexing.NDArrayIndex

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


	Public Class YoloUtils

		''' <summary>
		''' Essentially: just apply activation functions... For NCHW format. For NCHW format, use one of the other activate methods </summary>
		Public Shared Function activate(ByVal boundingBoxPriors As INDArray, ByVal input As INDArray) As INDArray
			Return activate(boundingBoxPriors, input, True)
		End Function

		Public Shared Function activate(ByVal boundingBoxPriors As INDArray, ByVal input As INDArray, ByVal nchw As Boolean) As INDArray
			Return activate(boundingBoxPriors, input, nchw, LayerWorkspaceMgr.noWorkspaces())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray activate(@NonNull INDArray boundingBoxPriors, @NonNull INDArray input, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr layerWorkspaceMgr)
		Public Shared Function activate(ByVal boundingBoxPriors As INDArray, ByVal input As INDArray, ByVal layerWorkspaceMgr As LayerWorkspaceMgr) As INDArray
			Return activate(boundingBoxPriors, input, True, layerWorkspaceMgr)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray activate(@NonNull INDArray boundingBoxPriors, @NonNull INDArray input, boolean nchw, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr layerWorkspaceMgr)
		Public Shared Function activate(ByVal boundingBoxPriors As INDArray, ByVal input As INDArray, ByVal nchw As Boolean, ByVal layerWorkspaceMgr As LayerWorkspaceMgr) As INDArray
			If Not nchw Then
				input = input.permute(0,3,1,2) 'NHWC to NCHW
			End If

			Dim mb As Long = input.size(0)
			Dim h As Long = input.size(2)
			Dim w As Long = input.size(3)
			Dim b As Long = boundingBoxPriors.size(0)
			Dim c As Long = input.size(1)\b-5 'input.size(1) == b * (5 + C) -> C = (input.size(1)/b) - 5

			Dim output As INDArray = layerWorkspaceMgr.create(ArrayType.ACTIVATIONS, input.dataType(), input.shape(), "c"c)
			Dim output5 As INDArray = output.reshape("c"c, mb, b, 5+c, h, w)
			Dim output4 As INDArray = output 'output.get(all(), interval(0,5*b), all(), all());
			Dim input4 As INDArray = input.dup("c"c) 'input.get(all(), interval(0,5*b), all(), all()).dup('c');
			Dim input5 As INDArray = input4.reshape("c"c, mb, b, 5+c, h, w)

			'X/Y center in grid: sigmoid
			Dim predictedXYCenterGrid As INDArray = input5.get(all(), all(), interval(0,2), all(), all())
			Transforms.sigmoid(predictedXYCenterGrid, False)

			'width/height: prior * exp(input)
			Dim predictedWHPreExp As INDArray = input5.get(all(), all(), interval(2,4), all(), all())
			Dim predictedWH As INDArray = Transforms.exp(predictedWHPreExp, False)
			Broadcast.mul(predictedWH, boundingBoxPriors.castTo(input.dataType()), predictedWH, 1, 2) 'Box priors: [b, 2]; predictedWH: [mb, b, 2, h, w]

			'Confidence - sigmoid
			Dim predictedConf As INDArray = input5.get(all(), all(), point(4), all(), all()) 'Shape: [mb, B, H, W]
			Transforms.sigmoid(predictedConf, False)

			output4.assign(input4)

			'Softmax
			'TODO OPTIMIZE?
			Dim inputClassesPreSoftmax As INDArray = input5.get(all(), all(), interval(5, 5+c), all(), all()) 'Shape: [minibatch, C, H, W]
			Dim classPredictionsPreSoftmax2d As INDArray = inputClassesPreSoftmax.permute(0,1,3,4,2).dup("c"c).reshape("c"c, New Long(){mb*b*h*w, c})
			Transforms.softmax(classPredictionsPreSoftmax2d, False)
			Dim postSoftmax5d As INDArray = classPredictionsPreSoftmax2d.reshape("c"c, mb, b, h, w, c).permute(0, 1, 4, 2, 3)

			Dim outputClasses As INDArray = output5.get(all(), all(), interval(5, 5+c), all(), all()) 'Shape: [minibatch, C, H, W]
			outputClasses.assign(postSoftmax5d)

			If Not nchw Then
				output = output.permute(0,2,3,1) 'NCHW to NHWC
			End If

			Return output
		End Function

		''' <summary>
		''' Returns overlap between lines [x1, x2] and [x3. x4]. </summary>
		Public Shared Function overlap(ByVal x1 As Double, ByVal x2 As Double, ByVal x3 As Double, ByVal x4 As Double) As Double
			If x3 < x1 Then
				If x4 < x1 Then
					Return 0
				Else
					Return Math.Min(x2, x4) - x1
				End If
			Else
				If x2 < x3 Then
					Return 0
				Else
					Return Math.Min(x2, x4) - x3
				End If
			End If
		End Function

		''' <summary>
		''' Returns intersection over union (IOU) between o1 and o2. </summary>
		Public Shared Function iou(ByVal o1 As DetectedObject, ByVal o2 As DetectedObject) As Double
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim x1min As Double = o1.getCenterX() - o1.getWidth() / 2
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim x1max As Double = o1.getCenterX() + o1.getWidth() / 2
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim y1min As Double = o1.getCenterY() - o1.getHeight() / 2
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim y1max As Double = o1.getCenterY() + o1.getHeight() / 2

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim x2min As Double = o2.getCenterX() - o2.getWidth() / 2
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim x2max As Double = o2.getCenterX() + o2.getWidth() / 2
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim y2min As Double = o2.getCenterY() - o2.getHeight() / 2
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim y2max As Double = o2.getCenterY() + o2.getHeight() / 2

			Dim ow As Double = overlap(x1min, x1max, x2min, x2max)
			Dim oh As Double = overlap(y1min, y1max, y2min, y2max)

			Dim intersection As Double = ow * oh
			Dim union As Double = o1.getWidth() * o1.getHeight() + o2.getWidth() * o2.getHeight() - intersection
			Return intersection / union
		End Function

		''' <summary>
		''' Performs non-maximum suppression (NMS) on objects, using their IOU with threshold to match pairs. </summary>
		Public Shared Sub nms(ByVal objects As IList(Of DetectedObject), ByVal iouThreshold As Double)
			Dim i As Integer = 0
			Do While i < objects.Count
				Dim j As Integer = 0
				Do While j < objects.Count
					Dim o1 As DetectedObject = objects(i)
					Dim o2 As DetectedObject = objects(j)
					If o1 IsNot Nothing AndAlso o2 IsNot Nothing AndAlso o1.PredictedClass = o2.PredictedClass AndAlso o1.getConfidence() < o2.getConfidence() AndAlso iou(o1, o2) > iouThreshold Then
						objects(i) = Nothing
					End If
					j += 1
				Loop
				i += 1
			Loop
			Dim it As IEnumerator(Of DetectedObject) = objects.GetEnumerator()
			Do While it.MoveNext()
				If it.Current Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: .NET enumerators are read-only:
					it.remove()
				End If
			Loop
		End Sub

		''' <summary>
		''' Given the network output and a detection threshold (in range 0 to 1) determine the objects detected by
		''' the network.<br>
		''' Supports minibatches - the returned <seealso cref="DetectedObject"/> instances have an example number index.<br>
		''' 
		''' Note that the dimensions are grid cell units - for example, with 416x416 input, 32x downsampling by the network
		''' (before getting to the Yolo2OutputLayer) we have 13x13 grid cells (each corresponding to 32 pixels in the input
		''' image). Thus, a centerX of 5.5 would be xPixels=5.5x32 = 176 pixels from left. Widths and heights are similar:
		''' in this example, a with of 13 would be the entire image (416 pixels), and a height of 6.5 would be 6.5/13 = 0.5
		''' of the image (208 pixels).
		''' </summary>
		''' <param name="boundingBoxPriors"> as given to Yolo2OutputLayer </param>
		''' <param name="networkOutput"> 4d activations out of the network </param>
		''' <param name="confThreshold"> Detection threshold, in range 0.0 (least strict) to 1.0 (most strict). Objects are returned
		'''                     where predicted confidence is >= confThreshold </param>
		''' <param name="nmsThreshold">  passed to <seealso cref="nms(List, Double)"/> (0 == disabled) as the threshold for intersection over union (IOU) </param>
		''' <returns> List of detected objects </returns>
		Public Shared Function getPredictedObjects(ByVal boundingBoxPriors As INDArray, ByVal networkOutput As INDArray, ByVal confThreshold As Double, ByVal nmsThreshold As Double) As IList(Of DetectedObject)
			If networkOutput.rank() <> 4 Then
				Throw New System.InvalidOperationException("Invalid network output activations array: should be rank 4. Got array " & "with shape " & Arrays.toString(networkOutput.shape()))
			End If
			If confThreshold < 0.0 OrElse confThreshold > 1.0 Then
				Throw New System.InvalidOperationException("Invalid confidence threshold: must be in range [0,1]. Got: " & confThreshold)
			End If

			'Activations format: [mb, 5b+c, h, w]
			Dim mb As Long = networkOutput.size(0)
			Dim h As Long = networkOutput.size(2)
			Dim w As Long = networkOutput.size(3)
			Dim b As Long = boundingBoxPriors.size(0)
			Dim c As Long = (networkOutput.size(1)\b)-5 'input.size(1) == b * (5 + C) -> C = (input.size(1)/b) - 5

			'Reshape from [minibatch, B*(5+C), H, W] to [minibatch, B, 5+C, H, W] to [minibatch, B, 5, H, W]
			Dim output5 As INDArray = networkOutput.dup("c"c).reshape(ChrW(mb), b, 5+c, h, w)
			Dim predictedConfidence As INDArray = output5.get(all(), all(), point(4), all(), all()) 'Shape: [mb, B, H, W]
			Dim softmax As INDArray = output5.get(all(), all(), interval(5, 5+c), all(), all())

			Dim [out] As IList(Of DetectedObject) = New List(Of DetectedObject)()
			For i As Integer = 0 To mb - 1
				For x As Integer = 0 To w - 1
					For y As Integer = 0 To h - 1
						For box As Integer = 0 To b - 1
							Dim conf As Double = predictedConfidence.getDouble(i, box, y, x)
							If conf < confThreshold Then
								Continue For
							End If

							Dim px As Double = output5.getDouble(i, box, 0, y, x) 'Originally: in 0 to 1 in grid cell
							Dim py As Double = output5.getDouble(i, box, 1, y, x) 'Originally: in 0 to 1 in grid cell
							Dim pw As Double = output5.getDouble(i, box, 2, y, x) 'In grid units (for example, 0 to 13)
							Dim ph As Double = output5.getDouble(i, box, 3, y, x) 'In grid units (for example, 0 to 13)

							'Convert the "position in grid cell" to "position in image (in grid cell units)"
							px += x
							py += y


							Dim sm As INDArray
							Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
								sm = softmax.get(point(i), point(box), all(), point(y), point(x)).dup()
							End Using

							[out].Add(New DetectedObject(i, px, py, pw, ph, sm, conf))
						Next box
					Next y
				Next x
			Next i

			If nmsThreshold > 0 Then
				nms([out], nmsThreshold)
			End If
			Return [out]
		End Function
	End Class

End Namespace