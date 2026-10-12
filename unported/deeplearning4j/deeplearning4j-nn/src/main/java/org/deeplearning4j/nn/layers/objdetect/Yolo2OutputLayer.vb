Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports IOutputLayer = org.deeplearning4j.nn.api.layers.IOutputLayer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports IsMax = org.nd4j.linalg.api.ops.impl.transforms.any.IsMax
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Broadcast = org.nd4j.linalg.factory.Broadcast
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossL2 = org.nd4j.linalg.lossfunctions.impl.LossL2
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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


	<Serializable>
	Public Class Yolo2OutputLayer
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.objdetect.Yolo2OutputLayer)
		Implements IOutputLayer

		Private Shared ReadOnly EMPTY_GRADIENT As Gradient = New DefaultGradient()

		'current input and label matrices
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected org.nd4j.linalg.api.ndarray.INDArray labels;
		Protected Friend labels As INDArray

		Private fullNetRegTerm As Double
'JAVA TO VB CONVERTER NOTE: The field score was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private score_Conflict As Double

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Dim epsOut As INDArray = computeBackpropGradientAndScore(workspaceMgr, False, False)

			Return New Pair(Of Gradient, INDArray)(EMPTY_GRADIENT, epsOut)
		End Function

		Private Function computeBackpropGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr, ByVal scoreOnly As Boolean, ByVal computeScoreForExamples As Boolean) As INDArray
			assertInputSet(True)
			Preconditions.checkState(labels IsNot Nothing, "Cannot calculate gradients/score: labels are null")
			Preconditions.checkState(labels.rank() = 4, "Expected rank 4 labels array with shape [minibatch, 4+numClasses, h, w]" & " but got rank %s labels array with shape %s", labels.rank(), labels.shape())

			Dim nchw As Boolean = layerConf().getFormat() = CNN2DFormat.NCHW
			Dim input As INDArray = If(nchw, Me.input_Conflict, Me.input_Conflict.permute(0,3,1,2)) 'NHWC to NCHW
			Dim labels As INDArray = Me.labels.castTo(input.dataType()) 'Ensure correct dtype (same as params); no-op if already correct dtype
			If Not nchw Then
				labels = labels.permute(0,3,1,2) 'NHWC to NCHW
			End If

			Dim lambdaCoord As Double = layerConf().getLambdaCoord()
			Dim lambdaNoObj As Double = layerConf().getLambdaNoObj()

			Dim mb As Long = input.size(0)
			Dim h As Long = input.size(2)
			Dim w As Long = input.size(3)
			Dim b As Integer = CInt(layerConf().getBoundingBoxes().size(0))
			Dim c As Integer = CInt(labels.size(1))-4



			'Various shape arrays, to reuse
			Dim nhw() As Long = {mb, h, w}

			'Labels shape: [mb, 4+C, H, W]
			'Infer mask array from labels. Mask array is 1_i^B in YOLO paper - i.e., whether an object is present in that
			' grid location or not. Here: we are using the fact that class labels are one-hot, and assume that values are
			' all 0s if no class label is present
			Preconditions.checkState(labels.rank() = 4, "Expected labels array to be rank 4 with shape [minibatch, 4+numClasses, H, W]. Got labels array with shape %ndShape", labels)
			Preconditions.checkState(labels.size(1) > 0, "Invalid labels array: labels.size(1) must be > 4. labels array should be rank 4 with shape [minibatch, 4+numClasses, H, W]. Got labels array with shape %ndShape", labels)

			Dim size1 As val = labels.size(1)
			Dim classLabels As INDArray = labels.get(all(), interval(4,size1), all(), all()) 'Shape: [minibatch, nClasses, H, W]
			Dim maskObjectPresent As INDArray = classLabels.sum(Nd4j.createUninitialized(input.dataType(), nhw, "c"c), 1) '.castTo(DataType.BOOL); //Shape: [minibatch, H, W]
			Dim maskObjectPresentBool As INDArray = maskObjectPresent.castTo(DataType.BOOL)

			' ----- Step 1: Labels format conversion -----
			'First: Convert labels/ground truth (x1,y1,x2,y2) from "coordinates (grid box units)" format to "center position in grid box" format
			'0.5 * ([x1,y1]+[x2,y2])   ->      shape: [mb, B, 2, H, W]
			Dim labelTLXY As INDArray = labels.get(all(), interval(0,2), all(), all())
			Dim labelBRXY As INDArray = labels.get(all(), interval(2,4), all(), all())

			Dim labelCenterXY As INDArray = labelTLXY.add(labelBRXY).muli(0.5) 'In terms of grid units
			Dim labelsCenterXYInGridBox As INDArray = labelCenterXY.dup(labelCenterXY.ordering()) '[mb, 2, H, W]
			labelsCenterXYInGridBox.subi(Transforms.floor(labelsCenterXYInGridBox,True))

			'Also infer size/scale (label w/h) from (x1,y1,x2,y2) format to (w,h) format
			Dim labelWHSqrt As INDArray = labelBRXY.sub(labelTLXY)
			labelWHSqrt = Transforms.sqrt(labelWHSqrt, False)



			' ----- Step 2: apply activation functions to network output activations -----
			'Reshape from [minibatch, B*(5+C), H, W] to [minibatch, B, 5+C, H, W]
			Dim expInputShape() As Long = {mb, b*(5+c), h, w}
			Dim newShape() As Long = {mb, b, 5+c, h, w}
			Dim newLength As Long = ArrayUtil.prodLong(newShape)
			Preconditions.checkState(expInputShape.SequenceEqual(input.shape()), "Unable to reshape input - input array shape does not match" & " expected shape. Expected input shape [minibatch, B*(5+C), H, W]=%s but got input of shape %ndShape. This may be due to an incorrect nOut (layer size/channels)" & " for the last convolutional layer in the network. nOut of the last layer must be B*(5+C) where B is the number of" & " bounding boxes, and C is the number of object classes. Expected B=%s from network configuration and C=%s from labels", expInputShape, input, b, c)
			Dim input5 As INDArray = input.dup("c"c).reshape("c"c, mb, b, 5+c, h, w)
			Dim inputClassesPreSoftmax As INDArray = input5.get(all(), all(), interval(5, 5+c), all(), all())

			' Sigmoid for x/y centers
			Dim preSigmoidPredictedXYCenterGrid As INDArray = input5.get(all(), all(), interval(0,2), all(), all())
			Dim predictedXYCenterGrid As INDArray = Transforms.sigmoid(preSigmoidPredictedXYCenterGrid, True) 'Not in-place, need pre-sigmoid later

			'Exponential for w/h (for: boxPrior * exp(input))      ->      Predicted WH in grid units (0 to 13 usually)
			Dim predictedWHPreExp As INDArray = input5.get(all(), all(), interval(2,4), all(), all())
			Dim predictedWH As INDArray = Transforms.exp(predictedWHPreExp, True)
			Broadcast.mul(predictedWH, layerConf().getBoundingBoxes().castTo(predictedWH.dataType()), predictedWH, 1, 2) 'Box priors: [b, 2]; predictedWH: [mb, b, 2, h, w]

			'Apply sqrt to W/H in preparation for loss function
			Dim predictedWHSqrt As INDArray = Transforms.sqrt(predictedWH, True)



			' ----- Step 3: Calculate IOU(predicted, labels) to infer 1_ij^obj mask array (for loss function) -----
			'Calculate IOU (intersection over union - aka Jaccard index) - for the labels and predicted values
			Dim iouRet As IOURet = calculateIOULabelPredicted(labelTLXY, labelBRXY, predictedWH, predictedXYCenterGrid, maskObjectPresent, maskObjectPresentBool) 'IOU shape: [minibatch, B, H, W]
			Dim iou As INDArray = iouRet.getIou()

			'Mask 1_ij^obj: isMax (dimension 1) + apply object present mask. Result: [minibatch, B, H, W]
			'In this mask: 1 if (a) object is present in cell [for each mb/H/W], AND (b) it is the box with the highest
			' IOU of any in the grid cell
			'We also need 1_ij^noobj, which is (a) no object, or (b) object present in grid cell, but this box doesn't
			' have the highest IOU
			Dim mask1_ij_obj As INDArray = Nd4j.create(DataType.BOOL, iou.shape(), "c"c)
			Nd4j.exec(New IsMax(iou, mask1_ij_obj, 1))
			Nd4j.exec(New BroadcastMulOp(mask1_ij_obj, maskObjectPresentBool, mask1_ij_obj, 0,2,3))
			Dim mask1_ij_noobj As INDArray = Transforms.not(mask1_ij_obj)
			mask1_ij_obj = mask1_ij_obj.castTo(input.dataType())



			' ----- Step 4: Calculate confidence, and confidence label -----
			'Predicted confidence: sigmoid (0 to 1)
			'Label confidence: 0 if no object, IOU(predicted,actual) if an object is present
			Dim labelConfidence As INDArray = iou.mul(mask1_ij_obj) 'Need to reuse IOU array later. IOU Shape: [mb, B, H, W]
			Dim predictedConfidencePreSigmoid As INDArray = input5.get(all(), all(), point(4), all(), all()) 'Shape: [mb, B, H, W]
			Dim predictedConfidence As INDArray = Transforms.sigmoid(predictedConfidencePreSigmoid, True)



			' ----- Step 5: Loss Function -----
			'One design goal here is to make the loss function configurable. To do this, we want to reshape the activations
			'(and masks) to a 2d representation, suitable for use in DL4J's loss functions

			Dim mask1_ij_obj_2d As INDArray = mask1_ij_obj.reshape(ChrW(mb*b*h*w), 1) 'Must be C order before reshaping
			Dim mask1_ij_noobj_2d As INDArray = mask1_ij_obj_2d.rsub(1.0)


			Dim predictedXYCenter2d As INDArray = predictedXYCenterGrid.permute(0,1,3,4,2).dup("c"c).reshape("c"c, mb*b*h*w, 2)
			'Don't use INDArray.broadcast(int...) until ND4J issue is fixed: https://github.com/deeplearning4j/nd4j/issues/2066
			'INDArray labelsCenterXYInGridBroadcast = labelsCenterXYInGrid.broadcast(mb, b, 2, h, w);
			'Broadcast labelsCenterXYInGrid from [mb, 2, h, w} to [mb, b, 2, h, w]
			Dim labelsCenterXYInGridBroadcast As INDArray = Nd4j.createUninitialized(input.dataType(), New Long(){mb, b, 2, h, w}, "c"c)
			For i As Integer = 0 To b - 1
				labelsCenterXYInGridBroadcast.get(all(), point(i), all(), all(), all()).assign(labelsCenterXYInGridBox)
			Next i
			Dim labelXYCenter2d As INDArray = labelsCenterXYInGridBroadcast.permute(0,1,3,4,2).dup("c"c).reshape("c"c, mb*b*h*w, 2) '[mb, b, 2, h, w] to [mb, b, h, w, 2] to [mb*b*h*w, 2]

			'Width/height (sqrt)
			Dim predictedWHSqrt2d As INDArray = predictedWHSqrt.permute(0,1,3,4,2).dup("c"c).reshape(mb*b*h*w, 2).dup("c"c) 'from [mb, b, 2, h, w] to [mb, b, h, w, 2] to [mb*b*h*w, 2]
			'Broadcast labelWHSqrt from [mb, 2, h, w} to [mb, b, 2, h, w]
			Dim labelWHSqrtBroadcast As INDArray = Nd4j.createUninitialized(input.dataType(), New Long(){mb, b, 2, h, w}, "c"c)
			For i As Integer = 0 To b - 1
				labelWHSqrtBroadcast.get(all(), point(i), all(), all(), all()).assign(labelWHSqrt) '[mb, 2, h, w] to [mb, b, 2, h, w]
			Next i
			Dim labelWHSqrt2d As INDArray = labelWHSqrtBroadcast.permute(0,1,3,4,2).dup("c"c).reshape(mb*b*h*w, 2).dup("c"c) '[mb, b, 2, h, w] to [mb, b, h, w, 2] to [mb*b*h*w, 2]

			'Confidence
			Dim labelConfidence2d As INDArray = labelConfidence.dup("c"c).reshape("c"c, mb * b * h * w, 1)
			Dim predictedConfidence2d As INDArray = predictedConfidence.dup("c"c).reshape("c"c, mb * b * h * w, 1).dup("c"c)
			Dim predictedConfidence2dPreSigmoid As INDArray = predictedConfidencePreSigmoid.dup("c"c).reshape("c"c, mb * b * h * w, 1).dup("c"c)


			'Class prediction loss
			Dim classPredictionsPreSoftmax2d As INDArray = inputClassesPreSoftmax.permute(0,1,3,4,2).dup("c"c).reshape("c"c, New Long(){mb*b*h*w, c})
			Dim classLabelsBroadcast As INDArray = Nd4j.createUninitialized(input.dataType(), New Long(){mb, b, c, h, w}, "c"c)
			For i As Integer = 0 To b - 1
				classLabelsBroadcast.get(all(), point(i), all(), all(), all()).assign(classLabels) '[mb, c, h, w] to [mb, b, c, h, w]
			Next i
			Dim classLabels2d As INDArray = classLabelsBroadcast.permute(0,1,3,4,2).dup("c"c).reshape("c"c, New Long(){mb*b*h*w, c})

			'Calculate the loss:
			Dim lossConfidence As ILossFunction = New LossL2()
			Dim identity As IActivation = New ActivationIdentity()


			If computeScoreForExamples Then
				Dim positionLoss As INDArray = layerConf().getLossPositionScale().computeScoreArray(labelXYCenter2d, predictedXYCenter2d, identity, mask1_ij_obj_2d)
				Dim sizeScaleLoss As INDArray = layerConf().getLossPositionScale().computeScoreArray(labelWHSqrt2d, predictedWHSqrt2d, identity, mask1_ij_obj_2d)
				Dim confidenceLossPt1 As INDArray = lossConfidence.computeScoreArray(labelConfidence2d, predictedConfidence2d, identity, mask1_ij_obj_2d)
				Dim confidenceLossPt2 As INDArray = lossConfidence.computeScoreArray(labelConfidence2d, predictedConfidence2d, identity, mask1_ij_noobj_2d).muli(lambdaNoObj)
				Dim classPredictionLoss As INDArray = layerConf().getLossClassPredictions().computeScoreArray(classLabels2d, classPredictionsPreSoftmax2d, New ActivationSoftmax(), mask1_ij_obj_2d)

				Dim scoreForExamples As INDArray = positionLoss.addi(sizeScaleLoss).muli(lambdaCoord).addi(confidenceLossPt1).addi(confidenceLossPt2.muli(lambdaNoObj)).addi(classPredictionLoss).dup("c"c)

				scoreForExamples = scoreForExamples.reshape("c"c, mb, b*h*w).sum(True, 1)
				If fullNetRegTerm > 0.0 Then
					scoreForExamples.addi(fullNetRegTerm)
				End If

				Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, scoreForExamples)
			End If

			Dim positionLoss As Double = layerConf().getLossPositionScale().computeScore(labelXYCenter2d, predictedXYCenter2d, identity, mask1_ij_obj_2d, False)
			Dim sizeScaleLoss As Double = layerConf().getLossPositionScale().computeScore(labelWHSqrt2d, predictedWHSqrt2d, identity, mask1_ij_obj_2d, False)
			Dim confidenceLoss As Double = lossConfidence.computeScore(labelConfidence2d, predictedConfidence2d, identity, mask1_ij_obj_2d, False) + lambdaNoObj * lossConfidence.computeScore(labelConfidence2d, predictedConfidence2d, identity, mask1_ij_noobj_2d, False) 'TODO: possible to optimize this?
			Dim classPredictionLoss As Double = layerConf().getLossClassPredictions().computeScore(classLabels2d, classPredictionsPreSoftmax2d, New ActivationSoftmax(), mask1_ij_obj_2d, False)

			Me.score_Conflict = lambdaCoord * (positionLoss + sizeScaleLoss) + confidenceLoss + classPredictionLoss

			Me.score_Conflict /= InputMiniBatchSize

			Me.score_Conflict += fullNetRegTerm

			If scoreOnly Then
				Return Nothing
			End If


			'==============================================================
			' ----- Gradient Calculation (specifically: return dL/dIn -----

			Dim epsOut As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), input.shape(), "c"c)
			Dim epsOut5 As INDArray = Shape.newShapeNoCopy(epsOut, New Long(){mb, b, 5+c, h, w}, False)
			Dim epsClassPredictions As INDArray = epsOut5.get(all(), all(), interval(5, 5+c), all(), all()) 'Shape: [mb, b, 5+c, h, w]
			Dim epsXY As INDArray = epsOut5.get(all(), all(), interval(0,2), all(), all())
			Dim epsWH As INDArray = epsOut5.get(all(), all(), interval(2,4), all(), all())
			Dim epsC As INDArray = epsOut5.get(all(), all(), point(4), all(), all())


			'Calculate gradient component from class probabilities (softmax)
			'Shape: [minibatch*h*w, c]
			Dim gradPredictionLoss2d As INDArray = layerConf().getLossClassPredictions().computeGradient(classLabels2d, classPredictionsPreSoftmax2d, New ActivationSoftmax(), mask1_ij_obj_2d)
			Dim gradPredictionLoss5d As INDArray = gradPredictionLoss2d.dup("c"c).reshape(ChrW(mb), b, h, w, c).permute(0,1,4,2,3).dup("c"c)
			epsClassPredictions.assign(gradPredictionLoss5d)


			'Calculate gradient component from position (x,y) loss - dL_position/dx and dL_position/dy
			Dim gradXYCenter2d As INDArray = layerConf().getLossPositionScale().computeGradient(labelXYCenter2d, predictedXYCenter2d, identity, mask1_ij_obj_2d)
			gradXYCenter2d.muli(lambdaCoord)
			Dim gradXYCenter5d As INDArray = gradXYCenter2d.dup("c"c).reshape("c"c, mb, b, h, w, 2).permute(0,1,4,2,3) 'From: [mb, B, H, W, 2] to [mb, B, 2, H, W]
			gradXYCenter5d = (New ActivationSigmoid()).backprop(preSigmoidPredictedXYCenterGrid.dup(), gradXYCenter5d).First
			epsXY.assign(gradXYCenter5d)

			'Calculate gradient component from width/height (w,h) loss - dL_size/dW and dL_size/dW
			'Note that loss function gets sqrt(w) and sqrt(h)
			'gradWHSqrt2d = dL/dsqrt(w) and dL/dsqrt(h)
			Dim gradWHSqrt2d As INDArray = layerConf().getLossPositionScale().computeGradient(labelWHSqrt2d, predictedWHSqrt2d, identity, mask1_ij_obj_2d) 'Shape: [mb*b*h*w, 2]
				'dL/dW = dL/dsqrtw * dsqrtw / dW = dL/dsqrtw * 0.5 / sqrt(w)
			Dim gradWH2d As INDArray = gradWHSqrt2d.muli(0.5).divi(predictedWHSqrt2d) 'dL/dW and dL/dH, w = pw * exp(tw)
				'dL/dinWH = dL/dW * dW/dInWH = dL/dW * pw * exp(tw)
			Dim gradWH5d As INDArray = gradWH2d.dup("c"c).reshape(ChrW(mb), b, h, w, 2).permute(0,1,4,2,3) 'To: [mb, b, 2, h, w]
			gradWH5d.muli(predictedWH)
			gradWH5d.muli(lambdaCoord)
			epsWH.assign(gradWH5d)


			'Calculate gradient component from confidence loss... 2 parts (object present, no object present)
			Dim gradConfidence2dA As INDArray = lossConfidence.computeGradient(labelConfidence2d, predictedConfidence2d, identity, mask1_ij_obj_2d)
			Dim gradConfidence2dB As INDArray = lossConfidence.computeGradient(labelConfidence2d, predictedConfidence2d, identity, mask1_ij_noobj_2d)


			Dim dLc_dC_2d As INDArray = gradConfidence2dA.addi(gradConfidence2dB.muli(lambdaNoObj)) 'dL/dC; C = sigmoid(tc)
			Dim dLc_dzc_2d As INDArray = (New ActivationSigmoid()).backprop(predictedConfidence2dPreSigmoid, dLc_dC_2d).First
			'Calculate dL/dtc
			Dim epsConfidence4d As INDArray = dLc_dzc_2d.dup("c"c).reshape("c"c, mb, b, h, w) '[mb*b*h*w, 2] to [mb, b, h, w]
			epsC.assign(epsConfidence4d)





			'Note that we ALSO have components to x,y,w,h  from confidence loss (via IOU, which depends on all of these values)
			'that is: dLc/dx, dLc/dy, dLc/dW, dLc/dH
			'For any value v, d(I/U)/dv = (U * dI/dv + I * dU/dv) / U^2

			'Confidence loss: sum squared errors + masking.
			'C == IOU when label present

			'Lc = 1^(obj)*(iou - predicted)^2 + lambdaNoObj * 1^(noobj) * (iou - predicted)^2 -> dLc/diou = 2*1^(obj)*(iou-predicted) + 2 * lambdaNoObj * 1^(noobj) * (iou-predicted) = 2*(iou-predicted) * (1^(obj) + lambdaNoObj * 1^(noobj))
			Dim twoIOUSubPredicted As INDArray = iou.subi(predictedConfidence).muli(2.0) 'Shape: [mb, b, h, w]. Note that when an object is present, IOU and confidence are the same. In-place to avoid copy op (iou no longer needed)
			Dim dLc_dIOU As INDArray = twoIOUSubPredicted.muli(mask1_ij_noobj.castTo(input.dataType()).muli(lambdaNoObj).addi(mask1_ij_obj))


			Dim dLc_dxy As INDArray = Nd4j.createUninitialized(iouRet.dIOU_dxy.dataType(), iouRet.dIOU_dxy.shape(), iouRet.dIOU_dxy.ordering())
			Broadcast.mul(iouRet.dIOU_dxy, dLc_dIOU, dLc_dxy, 0, 1, 3, 4) '[mb, b, h, w] x [mb, b, 2, h, w]

			Dim dLc_dwh As INDArray = Nd4j.createUninitialized(iouRet.dIOU_dwh.dataType(), iouRet.dIOU_dwh.shape(), iouRet.dIOU_dwh.ordering())
			Broadcast.mul(iouRet.dIOU_dwh, dLc_dIOU, dLc_dwh, 0, 1, 3, 4) '[mb, b, h, w] x [mb, b, 2, h, w]


			'Backprop through the wh and xy activation functions...
			'dL/dW and dL/dH, w = pw * exp(tw), //dL/dinWH = dL/dW * dW/dInWH = dL/dW * pw * exp(in_w)
			'as w = pw * exp(in_w) and dW/din_w = w
			Dim dLc_din_wh As INDArray = dLc_dwh.muli(predictedWH)
			Dim dLc_din_xy As INDArray = (New ActivationSigmoid()).backprop(preSigmoidPredictedXYCenterGrid, dLc_dxy).First 'Shape: same as subset of input... [mb, b, 2, h, w]

			'Finally, apply masks: dLc_dwh and dLc_dxy should be 0 if no object is present in that box
			'Apply mask 1^obj_ij with shape [mb, b, h, w]
			Broadcast.mul(dLc_din_wh, mask1_ij_obj, dLc_din_wh, 0, 1, 3, 4)
			Broadcast.mul(dLc_din_xy, mask1_ij_obj, dLc_din_xy, 0, 1, 3, 4)


			epsWH.addi(dLc_din_wh)
			epsXY.addi(dLc_din_xy)

			If Not nchw Then
				epsOut = epsOut.permute(0,2,3,1) 'NCHW to NHWC
			End If

			Return epsOut
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			Dim nchw As Boolean = layerConf().getFormat() = CNN2DFormat.NCHW
			Return YoloUtils.activate(layerConf().getBoundingBoxes(), input_Conflict, nchw, workspaceMgr)
		End Function

		Public Overrides Function clone() As Layer
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overridable Function needsLabels() As Boolean Implements IOutputLayer.needsLabels
			Return True
		End Function

		Public Overridable Function computeScore(ByVal fullNetRegTerm As Double, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Double Implements IOutputLayer.computeScore
			Me.fullNetRegTerm = fullNetRegTerm

			computeBackpropGradientAndScore(workspaceMgr, True, False)
			Return score()
		End Function

		Public Overrides Function score() As Double
			Return score_Conflict
		End Function

		''' <summary>
		''' Calculate IOU(truth, predicted) and gradients. Returns 5d arrays [mb, b, 2, H, W]
		''' ***NOTE: All labels - and predicted values - are in terms of grid units - 0 to 12 usually, with default config ***
		''' </summary>
		''' <param name="labelTL">   4d [mb, 2, H, W], label top/left (x,y) in terms of grid boxes </param>
		''' <param name="labelBR">   4d [mb, 2, H, W], label bottom/right (x,y) in terms of grid boxes </param>
		''' <param name="predictedWH"> 5d [mb, b, 2, H, W] - predicted H/W in terms of number of grid boxes. </param>
		''' <param name="predictedXYinGridBox"> 5d [mb, b, 2, H, W] - predicted X/Y in terms of number of grid boxes. Values 0 to 1, center box value being 0.5 </param>
		''' <param name="objectPresentMask"> 3d [mb, H, W] - mask array, for objects present (1) or not (0) in grid cell </param>
		''' <returns> IOU and gradients </returns>
		Private Shared Function calculateIOULabelPredicted(ByVal labelTL As INDArray, ByVal labelBR As INDArray, ByVal predictedWH As INDArray, ByVal predictedXYinGridBox As INDArray, ByVal objectPresentMask As INDArray, ByVal objectPresentMaskBool As INDArray) As IOURet

			Dim mb As Long = labelTL.size(0)
			Dim h As Long = labelTL.size(2)
			Dim w As Long = labelTL.size(3)
			Dim b As Long = predictedWH.size(1)

			Dim labelWH As INDArray = labelBR.sub(labelTL) '4d [mb, 2, H, W], label W/H in terms of number of grid boxes

			Dim gridH As Long = labelTL.size(2)
			Dim gridW As Long = labelTL.size(3)
			'Add grid positions to the predicted XY values (to get predicted XY in terms of grid cell units in image,
			' from (0 to 1 in grid cell) format)
			Dim linspaceX As INDArray = Nd4j.linspace(0, gridW-1, gridW, predictedWH.dataType())
			Dim linspaceY As INDArray = Nd4j.linspace(0, gridH-1, gridH, predictedWH.dataType())
			Dim grid As INDArray = Nd4j.createUninitialized(predictedWH.dataType(), New Long(){2, gridH, gridW}, "c"c)
			Dim gridX As INDArray = grid.get(point(0), all(), all())
			Dim gridY As INDArray = grid.get(point(1), all(), all())
			Broadcast.copy(gridX, linspaceX, gridX, 1)
			Broadcast.copy(gridY, linspaceY, gridY, 0)

			'Calculate X/Y position overall (in grid box units) from "position in current grid box" format
			Dim predictedXY As INDArray = predictedXYinGridBox.ulike()
			Broadcast.add(predictedXYinGridBox, grid, predictedXY, 2,3,4) ' [2, H, W] to [mb, b, 2, H, W]


			Dim halfWH As INDArray = predictedWH.mul(0.5)
			Dim predictedTL_XY As INDArray = halfWH.rsub(predictedXY) 'xy - 0.5 * wh
			Dim predictedBR_XY As INDArray = halfWH.add(predictedXY) 'xy + 0.5 * wh

			Dim maxTL As INDArray = predictedTL_XY.ulike() 'Shape: [mb, b, 2, H, W]
			Broadcast.max(predictedTL_XY, labelTL, maxTL, 0, 2, 3, 4)
			Dim minBR As INDArray = predictedBR_XY.ulike()
			Broadcast.min(predictedBR_XY, labelBR, minBR, 0, 2, 3, 4)

			Dim diff As INDArray = minBR.sub(maxTL)
			Dim intersectionArea As INDArray = diff.prod(2) '[mb, b, 2, H, W] to [mb, b, H, W]
			Broadcast.mul(intersectionArea, objectPresentMask, intersectionArea, 0, 2, 3)

			'Need to mask the calculated intersection values, to avoid returning non-zero values when intersection is actually 0
			'No intersection if: xP + wP/2 < xL - wL/2 i.e., BR_xPred < TL_xLab   OR  TL_xPred > BR_xLab (similar for Y axis)
			'Here, 1 if intersection exists, 0 otherwise. This is doing x/w and y/h simultaneously
			Dim noIntMask1 As INDArray = Nd4j.createUninitialized(DataType.BOOL, maxTL.shape(), maxTL.ordering())
			Dim noIntMask2 As INDArray = Nd4j.createUninitialized(DataType.BOOL, maxTL.shape(), maxTL.ordering())
			'Does both x and y on different dims
			Broadcast.lte(predictedBR_XY, labelTL, noIntMask1, 0, 2, 3, 4) 'Predicted BR <= label TL
			Broadcast.gte(predictedTL_XY, labelBR, noIntMask2, 0, 2, 3, 4) 'predicted TL >= label BR

			noIntMask1 = Transforms.or(noIntMask1.get(all(), all(), point(0), all(), all()), noIntMask1.get(all(), all(), point(1), all(), all())) 'Shape: [mb, b, H, W]. Values 1 if no intersection
			noIntMask2 = Transforms.or(noIntMask2.get(all(), all(), point(0), all(), all()), noIntMask2.get(all(), all(), point(1), all(), all()))
			Dim noIntMask As INDArray = Transforms.or(noIntMask1, noIntMask2)

			Dim intMask As INDArray = Transforms.not(noIntMask) 'Values 0 if no intersection
			Broadcast.mul(intMask, objectPresentMaskBool, intMask, 0, 2, 3)

			'Mask the intersection area: should be 0 if no intersection
			intMask = intMask.castTo(predictedWH.dataType())
			intersectionArea.muli(intMask)


			'Next, union area is simple: U = A1 + A2 - intersection
			Dim areaPredicted As INDArray = predictedWH.prod(2) '[mb, b, 2, H, W] to [mb, b, H, W]
			Broadcast.mul(areaPredicted, objectPresentMask, areaPredicted, 0,2,3)
			Dim areaLabel As INDArray = labelWH.prod(1) '[mb, 2, H, W] to [mb, H, W]

			Dim unionArea As INDArray = Broadcast.add(areaPredicted, areaLabel, areaPredicted.dup(), 0, 2, 3)
			unionArea.subi(intersectionArea)
			unionArea.muli(intMask)

			Dim iou As INDArray = intersectionArea.div(unionArea)
			BooleanIndexing.replaceWhere(iou, 0.0, Conditions.Nan) '0/0 -> NaN -> 0

			'Apply the "object present" mask (of shape [mb, h, w]) - this ensures IOU is 0 if no object is present
			Broadcast.mul(iou, objectPresentMask, iou, 0, 2, 3)

			'Finally, calculate derivatives:
			Dim maskMaxTL As INDArray = Nd4j.createUninitialized(DataType.BOOL, maxTL.shape(), maxTL.ordering()) '1 if predicted Top/Left is max, 0 otherwise
			Broadcast.gt(predictedTL_XY, labelTL, maskMaxTL, 0, 2, 3, 4) ' z = x > y
			maskMaxTL = maskMaxTL.castTo(predictedWH.dataType())

			Dim maskMinBR As INDArray = Nd4j.createUninitialized(DataType.BOOL, maxTL.shape(), maxTL.ordering()) '1 if predicted Top/Left is max, 0 otherwise
			Broadcast.lt(predictedBR_XY, labelBR, maskMinBR, 0, 2, 3, 4) ' z = x < y
			maskMinBR = maskMinBR.castTo(predictedWH.dataType())

			'dI/dx = lambda * (1^(min(x1+w1/2) - 1^(max(x1-w1/2))
			'dI/dy = omega * (1^(min(y1+h1/2) - 1^(max(y1-h1/2))
			'omega = min(x1+w1/2,x2+w2/2) - max(x1-w1/2,x2+w2/2)       i.e., from diff = minBR.sub(maxTL), which has shape [mb, b, 2, h, w]
			'lambda = min(y1+h1/2,y2+h2/2) - max(y1-h1/2,y2+h2/2)
			Dim dI_dxy As INDArray = maskMinBR.sub(maskMaxTL) 'Shape: [mb, b, 2, h, w]
			Dim dI_dwh As INDArray = maskMinBR.addi(maskMaxTL).muli(0.5) 'Shape: [mb, b, 2, h, w]

			dI_dxy.get(all(), all(), point(0), all(), all()).muli(diff.get(all(), all(), point(1), all(), all()))
			dI_dxy.get(all(), all(), point(1), all(), all()).muli(diff.get(all(), all(), point(0), all(), all()))

			dI_dwh.get(all(), all(), point(0), all(), all()).muli(diff.get(all(), all(), point(1), all(), all()))
			dI_dwh.get(all(), all(), point(1), all(), all()).muli(diff.get(all(), all(), point(0), all(), all()))

			'And derivatives WRT IOU:
			Dim uPlusI As INDArray = unionArea.add(intersectionArea)
			Dim u2 As INDArray = unionArea.mul(unionArea)
			Dim uPlusIDivU2 As INDArray = uPlusI.div(u2) 'Shape: [mb, b, h, w]
			BooleanIndexing.replaceWhere(uPlusIDivU2, 0.0, Conditions.Nan) 'Handle 0/0

			Dim dIOU_dxy As INDArray = Nd4j.createUninitialized(predictedWH.dataType(), New Long(){mb, b, 2, h, w}, "c"c)
			Broadcast.mul(dI_dxy, uPlusIDivU2, dIOU_dxy, 0, 1, 3, 4) '[mb, b, h, w] x [mb, b, 2, h, w]

			Dim predictedHW As INDArray = Nd4j.createUninitialized(predictedWH.dataType(), New Long(){mb, b, 2, h, w}, predictedWH.ordering())
			'Next 2 lines: permuting the order... WH to HW along dimension 2
			predictedHW.get(all(), all(), point(0), all(), all()).assign(predictedWH.get(all(), all(), point(1), all(), all()))
			predictedHW.get(all(), all(), point(1), all(), all()).assign(predictedWH.get(all(), all(), point(0), all(), all()))

			Dim Ihw As INDArray = predictedHW.ulike()
			Broadcast.mul(predictedHW, intersectionArea, Ihw, 0, 1, 3, 4) 'Predicted_wh: [mb, b, 2, h, w]; intersection: [mb, b, h, w]

			Dim dIOU_dwh As INDArray = Nd4j.createUninitialized(predictedHW.dataType(), New Long(){mb, b, 2, h, w}, "c"c)
			Broadcast.mul(dI_dwh, uPlusI, dIOU_dwh, 0, 1, 3, 4)
			dIOU_dwh.subi(Ihw)
			Broadcast.div(dIOU_dwh, u2, dIOU_dwh, 0, 1, 3, 4)
			BooleanIndexing.replaceWhere(dIOU_dwh, 0.0, Conditions.Nan) 'Handle division by 0 (due to masking, etc)

			Return New IOURet(iou, dIOU_dxy, dIOU_dwh)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data private static class IOURet
		Private Class IOURet
			Friend iou As INDArray
			Friend dIOU_dxy As INDArray
			Friend dIOU_dwh As INDArray

		End Class

		Public Overrides Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)

			'TODO
			Throw New System.NotSupportedException("Not yet implemented")
		End Sub

		Public Overrides Function gradientAndScore() As Pair(Of Gradient, Double)
			Return New Pair(Of Gradient, Double)(gradient(), score())
		End Function

		Public Overridable Function computeScoreForExamples(ByVal fullNetRegTerm As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IOutputLayer.computeScoreForExamples
			Me.fullNetRegTerm = fullNetRegTerm
			Return computeBackpropGradientAndScore(workspaceMgr, False, True)
		End Function

		Public Overridable Function f1Score(ByVal data As DataSet) As Double
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function f1Score(ByVal examples As INDArray, ByVal labels As INDArray) As Double
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function numLabels() As Integer
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Overloads Sub fit(ByVal iter As DataSetIterator)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Function predict(ByVal examples As INDArray) As Integer()
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function predict(ByVal dataSet As DataSet) As IList(Of String)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Overloads Sub fit(ByVal examples As INDArray, ByVal labels As INDArray)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Overloads Sub fit(ByVal data As DataSet)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Overloads Sub fit(ByVal examples As INDArray, ByVal labels() As Integer)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub clearNoiseWeightParams()
			'No op
		End Sub

		''' <seealso cref= YoloUtils#getPredictedObjects(INDArray, INDArray, double, double) </seealso>
		Public Overridable Function getPredictedObjects(ByVal networkOutput As INDArray, ByVal threshold As Double) As IList(Of DetectedObject)
			Return YoloUtils.getPredictedObjects(layerConf().getBoundingBoxes(), networkOutput, threshold, 0.0)
		End Function

		''' <summary>
		''' Get the confidence matrix (confidence for all x/y positions) for the specified bounding box, from the network
		''' output activations array
		''' </summary>
		''' <param name="networkOutput"> Network output activations </param>
		''' <param name="example">       Example number, in minibatch </param>
		''' <param name="bbNumber">      Bounding box number </param>
		''' <returns> Confidence matrix </returns>
		Public Overridable Function getConfidenceMatrix(ByVal networkOutput As INDArray, ByVal example As Integer, ByVal bbNumber As Integer) As INDArray

			'Input format: [minibatch, 5B+C, H, W], with order [x,y,w,h,c]
			'Therefore: confidences are at depths 4 + bbNumber * 5

			Dim conf As INDArray = networkOutput.get(point(example), point(4+bbNumber*5), all(), all())
			Return conf
		End Function

		''' <summary>
		''' Get the probability matrix (probability of the specified class, assuming an object is present, for all x/y
		''' positions), from the network output activations array
		''' </summary>
		''' <param name="networkOutput"> Network output activations </param>
		''' <param name="example">       Example number, in minibatch </param>
		''' <param name="classNumber">   Class number </param>
		''' <returns> Confidence matrix </returns>
		Public Overridable Function getProbabilityMatrix(ByVal networkOutput As INDArray, ByVal example As Integer, ByVal classNumber As Integer) As INDArray
			'Input format: [minibatch, 5B+C, H, W], with order [x,y,w,h,c]
			'Therefore: probabilities for class I is at depths 5B + classNumber

			Dim bbs As val = layerConf().getBoundingBoxes().size(0)
			Dim conf As INDArray = networkOutput.get(point(example), point(5*bbs + classNumber), all(), all())
			Return conf
		End Function
	End Class

End Namespace