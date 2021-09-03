import static org.nd4j.autodiff.samediff.ops.SDValidation.isSameType
Imports LossReduce = org.nd4j.autodiff.loss.LossReduce
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff

''' <summary>
'''*****************************************************************************
''' Copyright (c) 2019-2020 Konduit K.K.
''' 
''' This program and the accompanying materials are made available under the
''' terms of the Apache License, Version 2.0 which is available at
''' https://www.apache.org/licenses/LICENSE-2.0.
''' 
''' Unless required by applicable law or agreed to in writing, software
''' distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
''' WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
''' License for the specific language governing permissions and limitations
''' under the License.
''' 
''' SPDX-License-Identifier: Apache-2.0
''' *****************************************************************************
''' </summary>

'================== GENERATED CODE - DO NOT MODIFY THIS FILE ==================

Namespace org.nd4j.autodiff.samediff.ops

	Public Class SDLoss
		Inherits SDOps

	  Public Sub New(ByVal sameDiff As SameDiff)
		MyBase.New(sameDiff)
	  End Sub

	  ''' <summary>
	  ''' Absolute difference loss: {@code sum_i abs( label[i] - predictions[i] )}<br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <returns> output loss variable (NUMERIC type) </returns>
	  Public Overridable Function absoluteDifference(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce) As SDVariable
		SDValidation.validateNumerical("absoluteDifference", "label", label)
		SDValidation.validateNumerical("absoluteDifference", "predictions", predictions)
		SDValidation.validateNumerical("absoluteDifference", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.AbsoluteDifferenceLoss(sd,label, predictions, weights, lossReduce)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Absolute difference loss: {@code sum_i abs( label[i] - predictions[i] )}<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <returns> output loss variable (NUMERIC type) </returns>
	  Public Overridable Function absoluteDifference(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce) As SDVariable
		SDValidation.validateNumerical("absoluteDifference", "label", label)
		SDValidation.validateNumerical("absoluteDifference", "predictions", predictions)
		SDValidation.validateNumerical("absoluteDifference", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.AbsoluteDifferenceLoss(sd,label, predictions, weights, lossReduce)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute difference loss: {@code sum_i abs( label[i] - predictions[i] )}<br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output loss variable (NUMERIC type) </returns>
	  Public Overridable Function absoluteDifference(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("absoluteDifference", "label", label)
		SDValidation.validateNumerical("absoluteDifference", "predictions", predictions)
		SDValidation.validateNumerical("absoluteDifference", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.AbsoluteDifferenceLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Absolute difference loss: {@code sum_i abs( label[i] - predictions[i] )}<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output loss variable (NUMERIC type) </returns>
	  Public Overridable Function absoluteDifference(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("absoluteDifference", "label", label)
		SDValidation.validateNumerical("absoluteDifference", "predictions", predictions)
		SDValidation.validateNumerical("absoluteDifference", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.AbsoluteDifferenceLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Cosine distance loss: {@code 1 - cosineSimilarity(x,y)} or {@code 1 - sum_i label[i] * prediction[i]}, which is<br>
	  ''' equivalent to cosine distance when both the predictions and labels are normalized.<br>
	  ''' <b>Note</b>: This loss function assumes that both the predictions and labels are normalized to have unit l2 norm.<br>
	  ''' If this is not the case, you should normalize them first by dividing by norm2(String, SDVariable, boolean, int...)<br>
	  ''' along the cosine distance dimension (with keepDims=true).<br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is use (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="dimension"> Dimension to perform the cosine distance over </param>
	  ''' <returns> output Cosine distance loss  (NUMERIC type) </returns>
	  Public Overridable Function cosineDistance(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal dimension As Integer) As SDVariable
		SDValidation.validateNumerical("cosineDistance", "label", label)
		SDValidation.validateNumerical("cosineDistance", "predictions", predictions)
		SDValidation.validateNumerical("cosineDistance", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.CosineDistanceLoss(sd,label, predictions, weights, lossReduce, dimension)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Cosine distance loss: {@code 1 - cosineSimilarity(x,y)} or {@code 1 - sum_i label[i] * prediction[i]}, which is<br>
	  ''' equivalent to cosine distance when both the predictions and labels are normalized.<br>
	  ''' <b>Note</b>: This loss function assumes that both the predictions and labels are normalized to have unit l2 norm.<br>
	  ''' If this is not the case, you should normalize them first by dividing by norm2(String, SDVariable, boolean, int...)<br>
	  ''' along the cosine distance dimension (with keepDims=true).<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is use (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="dimension"> Dimension to perform the cosine distance over </param>
	  ''' <returns> output Cosine distance loss  (NUMERIC type) </returns>
	  Public Overridable Function cosineDistance(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal dimension As Integer) As SDVariable
		SDValidation.validateNumerical("cosineDistance", "label", label)
		SDValidation.validateNumerical("cosineDistance", "predictions", predictions)
		SDValidation.validateNumerical("cosineDistance", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.CosineDistanceLoss(sd,label, predictions, weights, lossReduce, dimension)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Cosine distance loss: {@code 1 - cosineSimilarity(x,y)} or {@code 1 - sum_i label[i] * prediction[i]}, which is<br>
	  ''' equivalent to cosine distance when both the predictions and labels are normalized.<br>
	  ''' <b>Note</b>: This loss function assumes that both the predictions and labels are normalized to have unit l2 norm.<br>
	  ''' If this is not the case, you should normalize them first by dividing by norm2(String, SDVariable, boolean, int...)<br>
	  ''' along the cosine distance dimension (with keepDims=true).<br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is use (NUMERIC type) </param>
	  ''' <param name="dimension"> Dimension to perform the cosine distance over </param>
	  ''' <returns> output Cosine distance loss  (NUMERIC type) </returns>
	  Public Overridable Function cosineDistance(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal dimension As Integer) As SDVariable
		SDValidation.validateNumerical("cosineDistance", "label", label)
		SDValidation.validateNumerical("cosineDistance", "predictions", predictions)
		SDValidation.validateNumerical("cosineDistance", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.CosineDistanceLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, dimension)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Cosine distance loss: {@code 1 - cosineSimilarity(x,y)} or {@code 1 - sum_i label[i] * prediction[i]}, which is<br>
	  ''' equivalent to cosine distance when both the predictions and labels are normalized.<br>
	  ''' <b>Note</b>: This loss function assumes that both the predictions and labels are normalized to have unit l2 norm.<br>
	  ''' If this is not the case, you should normalize them first by dividing by norm2(String, SDVariable, boolean, int...)<br>
	  ''' along the cosine distance dimension (with keepDims=true).<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is use (NUMERIC type) </param>
	  ''' <param name="dimension"> Dimension to perform the cosine distance over </param>
	  ''' <returns> output Cosine distance loss  (NUMERIC type) </returns>
	  Public Overridable Function cosineDistance(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal dimension As Integer) As SDVariable
		SDValidation.validateNumerical("cosineDistance", "label", label)
		SDValidation.validateNumerical("cosineDistance", "predictions", predictions)
		SDValidation.validateNumerical("cosineDistance", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.CosineDistanceLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, dimension)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' CTC Loss: Connectionist Temporal Classification Loss. See:<br>
	  ''' https://dl.acm.org/citation.cfm?id=1143891<br>
	  ''' </summary>
	  ''' <param name="targetLabels"> Label array (NUMERIC type) </param>
	  ''' <param name="logitInput"> Inputs (NUMERIC type) </param>
	  ''' <param name="targetLabelLengths"> Length of the target label (NUMERIC type) </param>
	  ''' <param name="logitInputLengths"> Length of the input (NUMERIC type) </param>
	  ''' <returns> output Ctc loss  (NUMERIC type) </returns>
	  Public Overridable Function ctcLoss(ByVal targetLabels As SDVariable, ByVal logitInput As SDVariable, ByVal targetLabelLengths As SDVariable, ByVal logitInputLengths As SDVariable) As SDVariable
		SDValidation.validateNumerical("ctcLoss", "targetLabels", targetLabels)
		SDValidation.validateNumerical("ctcLoss", "logitInput", logitInput)
		SDValidation.validateNumerical("ctcLoss", "targetLabelLengths", targetLabelLengths)
		SDValidation.validateNumerical("ctcLoss", "logitInputLengths", logitInputLengths)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.CtcLoss(sd,targetLabels, logitInput, targetLabelLengths, logitInputLengths)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' CTC Loss: Connectionist Temporal Classification Loss. See:<br>
	  ''' https://dl.acm.org/citation.cfm?id=1143891<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="targetLabels"> Label array (NUMERIC type) </param>
	  ''' <param name="logitInput"> Inputs (NUMERIC type) </param>
	  ''' <param name="targetLabelLengths"> Length of the target label (NUMERIC type) </param>
	  ''' <param name="logitInputLengths"> Length of the input (NUMERIC type) </param>
	  ''' <returns> output Ctc loss  (NUMERIC type) </returns>
	  Public Overridable Function ctcLoss(ByVal name As String, ByVal targetLabels As SDVariable, ByVal logitInput As SDVariable, ByVal targetLabelLengths As SDVariable, ByVal logitInputLengths As SDVariable) As SDVariable
		SDValidation.validateNumerical("ctcLoss", "targetLabels", targetLabels)
		SDValidation.validateNumerical("ctcLoss", "logitInput", logitInput)
		SDValidation.validateNumerical("ctcLoss", "targetLabelLengths", targetLabelLengths)
		SDValidation.validateNumerical("ctcLoss", "logitInputLengths", logitInputLengths)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.CtcLoss(sd,targetLabels, logitInput, targetLabelLengths, logitInputLengths)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Hinge loss: a loss function used for training classifiers.<br>
	  ''' Implements {@code L = max(0, 1 - t * predictions)} where t is the label values after internally converting to {-1,1}<br>
	  ''' from the user specified {0,1}. Note that Labels should be provided with values {0,1}.<br>
	  ''' </summary>
	  ''' <param name="label"> Label array. Each value should be 0.0 or 1.0 (internally -1 to 1 is used) (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function hingeLoss(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce) As SDVariable
		SDValidation.validateNumerical("hingeLoss", "label", label)
		SDValidation.validateNumerical("hingeLoss", "predictions", predictions)
		SDValidation.validateNumerical("hingeLoss", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.HingeLoss(sd,label, predictions, weights, lossReduce)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Hinge loss: a loss function used for training classifiers.<br>
	  ''' Implements {@code L = max(0, 1 - t * predictions)} where t is the label values after internally converting to {-1,1}<br>
	  ''' from the user specified {0,1}. Note that Labels should be provided with values {0,1}.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array. Each value should be 0.0 or 1.0 (internally -1 to 1 is used) (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function hingeLoss(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce) As SDVariable
		SDValidation.validateNumerical("hingeLoss", "label", label)
		SDValidation.validateNumerical("hingeLoss", "predictions", predictions)
		SDValidation.validateNumerical("hingeLoss", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.HingeLoss(sd,label, predictions, weights, lossReduce)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Hinge loss: a loss function used for training classifiers.<br>
	  ''' Implements {@code L = max(0, 1 - t * predictions)} where t is the label values after internally converting to {-1,1}<br>
	  ''' from the user specified {0,1}. Note that Labels should be provided with values {0,1}.<br>
	  ''' </summary>
	  ''' <param name="label"> Label array. Each value should be 0.0 or 1.0 (internally -1 to 1 is used) (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function hingeLoss(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("hingeLoss", "label", label)
		SDValidation.validateNumerical("hingeLoss", "predictions", predictions)
		SDValidation.validateNumerical("hingeLoss", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.HingeLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Hinge loss: a loss function used for training classifiers.<br>
	  ''' Implements {@code L = max(0, 1 - t * predictions)} where t is the label values after internally converting to {-1,1}<br>
	  ''' from the user specified {0,1}. Note that Labels should be provided with values {0,1}.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array. Each value should be 0.0 or 1.0 (internally -1 to 1 is used) (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function hingeLoss(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("hingeLoss", "label", label)
		SDValidation.validateNumerical("hingeLoss", "predictions", predictions)
		SDValidation.validateNumerical("hingeLoss", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.HingeLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Huber loss function, used for robust regression. It is similar both squared error loss and absolute difference loss,<br>
	  ''' though is less sensitive to outliers than squared error.<br>
	  ''' Huber loss implements:<br>
	  ''' <pre><br>
	  ''' {@code L = 0.5 * (label[i] - predictions[i])^2 if abs(label[i] - predictions[i]) < delta}<br>
	  ''' {@code L = delta * abs(label[i] - predictions[i]) - 0.5 * delta^2 otherwise}<br>
	  ''' </pre><br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="delta"> Loss function delta value </param>
	  ''' <returns> output Huber loss (NUMERIC type) </returns>
	  Public Overridable Function huberLoss(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal delta As Double) As SDVariable
		SDValidation.validateNumerical("huberLoss", "label", label)
		SDValidation.validateNumerical("huberLoss", "predictions", predictions)
		SDValidation.validateNumerical("huberLoss", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.HuberLoss(sd,label, predictions, weights, lossReduce, delta)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Huber loss function, used for robust regression. It is similar both squared error loss and absolute difference loss,<br>
	  ''' though is less sensitive to outliers than squared error.<br>
	  ''' Huber loss implements:<br>
	  ''' <pre><br>
	  ''' {@code L = 0.5 * (label[i] - predictions[i])^2 if abs(label[i] - predictions[i]) < delta}<br>
	  ''' {@code L = delta * abs(label[i] - predictions[i]) - 0.5 * delta^2 otherwise}<br>
	  ''' </pre><br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="delta"> Loss function delta value </param>
	  ''' <returns> output Huber loss (NUMERIC type) </returns>
	  Public Overridable Function huberLoss(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal delta As Double) As SDVariable
		SDValidation.validateNumerical("huberLoss", "label", label)
		SDValidation.validateNumerical("huberLoss", "predictions", predictions)
		SDValidation.validateNumerical("huberLoss", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.HuberLoss(sd,label, predictions, weights, lossReduce, delta)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Huber loss function, used for robust regression. It is similar both squared error loss and absolute difference loss,<br>
	  ''' though is less sensitive to outliers than squared error.<br>
	  ''' Huber loss implements:<br>
	  ''' <pre><br>
	  ''' {@code L = 0.5 * (label[i] - predictions[i])^2 if abs(label[i] - predictions[i]) < delta}<br>
	  ''' {@code L = delta * abs(label[i] - predictions[i]) - 0.5 * delta^2 otherwise}<br>
	  ''' </pre><br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="delta"> Loss function delta value </param>
	  ''' <returns> output Huber loss (NUMERIC type) </returns>
	  Public Overridable Function huberLoss(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal delta As Double) As SDVariable
		SDValidation.validateNumerical("huberLoss", "label", label)
		SDValidation.validateNumerical("huberLoss", "predictions", predictions)
		SDValidation.validateNumerical("huberLoss", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.HuberLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, delta)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Huber loss function, used for robust regression. It is similar both squared error loss and absolute difference loss,<br>
	  ''' though is less sensitive to outliers than squared error.<br>
	  ''' Huber loss implements:<br>
	  ''' <pre><br>
	  ''' {@code L = 0.5 * (label[i] - predictions[i])^2 if abs(label[i] - predictions[i]) < delta}<br>
	  ''' {@code L = delta * abs(label[i] - predictions[i]) - 0.5 * delta^2 otherwise}<br>
	  ''' </pre><br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="delta"> Loss function delta value </param>
	  ''' <returns> output Huber loss (NUMERIC type) </returns>
	  Public Overridable Function huberLoss(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal delta As Double) As SDVariable
		SDValidation.validateNumerical("huberLoss", "label", label)
		SDValidation.validateNumerical("huberLoss", "predictions", predictions)
		SDValidation.validateNumerical("huberLoss", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.HuberLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, delta)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' L2 loss: 1/2 * sum(x^2)<br>
	  ''' </summary>
	  ''' <param name="var"> Variable to calculate L2 loss of (NUMERIC type) </param>
	  ''' <returns> output L2 loss (NUMERIC type) </returns>
	  Public Overridable Function l2Loss(ByVal var As SDVariable) As SDVariable
		SDValidation.validateNumerical("l2Loss", "var", var)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.L2Loss(sd,var)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' L2 loss: 1/2 * sum(x^2)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="var"> Variable to calculate L2 loss of (NUMERIC type) </param>
	  ''' <returns> output L2 loss (NUMERIC type) </returns>
	  Public Overridable Function l2Loss(ByVal name As String, ByVal var As SDVariable) As SDVariable
		SDValidation.validateNumerical("l2Loss", "var", var)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.L2Loss(sd,var)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Log loss, i.e., binary cross entropy loss, usually used for binary multi-label classification. Implements:<br>
	  ''' {@code -1/numExamples * sum_i (labels[i] * log(predictions[i] + epsilon) + (1-labels[i]) * log(1-predictions[i] + epsilon))}<br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="epsilon"> epsilon </param>
	  ''' <returns> output Log loss  (NUMERIC type) </returns>
	  Public Overridable Function logLoss(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal epsilon As Double) As SDVariable
		SDValidation.validateNumerical("logLoss", "label", label)
		SDValidation.validateNumerical("logLoss", "predictions", predictions)
		SDValidation.validateNumerical("logLoss", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.LogLoss(sd,label, predictions, weights, lossReduce, epsilon)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Log loss, i.e., binary cross entropy loss, usually used for binary multi-label classification. Implements:<br>
	  ''' {@code -1/numExamples * sum_i (labels[i] * log(predictions[i] + epsilon) + (1-labels[i]) * log(1-predictions[i] + epsilon))}<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="epsilon"> epsilon </param>
	  ''' <returns> output Log loss  (NUMERIC type) </returns>
	  Public Overridable Function logLoss(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal epsilon As Double) As SDVariable
		SDValidation.validateNumerical("logLoss", "label", label)
		SDValidation.validateNumerical("logLoss", "predictions", predictions)
		SDValidation.validateNumerical("logLoss", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.LogLoss(sd,label, predictions, weights, lossReduce, epsilon)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Log loss, i.e., binary cross entropy loss, usually used for binary multi-label classification. Implements:<br>
	  ''' {@code -1/numExamples * sum_i (labels[i] * log(predictions[i] + epsilon) + (1-labels[i]) * log(1-predictions[i] + epsilon))}<br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <returns> output Log loss  (NUMERIC type) </returns>
	  Public Overridable Function logLoss(ByVal label As SDVariable, ByVal predictions As SDVariable) As SDVariable
		SDValidation.validateNumerical("logLoss", "label", label)
		SDValidation.validateNumerical("logLoss", "predictions", predictions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.LogLoss(sd,label, predictions, Nothing, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, 0.0)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Log loss, i.e., binary cross entropy loss, usually used for binary multi-label classification. Implements:<br>
	  ''' {@code -1/numExamples * sum_i (labels[i] * log(predictions[i] + epsilon) + (1-labels[i]) * log(1-predictions[i] + epsilon))}<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <returns> output Log loss  (NUMERIC type) </returns>
	  Public Overridable Function logLoss(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable) As SDVariable
		SDValidation.validateNumerical("logLoss", "label", label)
		SDValidation.validateNumerical("logLoss", "predictions", predictions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.LogLoss(sd,label, predictions, Nothing, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, 0.0)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Log poisson loss: a loss function used for training classifiers.<br>
	  ''' Implements {@code L = exp(c) - z * c} where c is log(predictions) and z is labels.<br>
	  ''' </summary>
	  ''' <param name="label"> Label array. Each value should be 0.0 or 1.0 (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (has to be log(x) of actual predictions) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="full"> Boolean flag. true for logPoissonFull, false for logPoisson </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function logPoisson(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal full As Boolean) As SDVariable
		SDValidation.validateNumerical("logPoisson", "label", label)
		SDValidation.validateNumerical("logPoisson", "predictions", predictions)
		SDValidation.validateNumerical("logPoisson", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.LogPoissonLoss(sd,label, predictions, weights, lossReduce, full)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Log poisson loss: a loss function used for training classifiers.<br>
	  ''' Implements {@code L = exp(c) - z * c} where c is log(predictions) and z is labels.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array. Each value should be 0.0 or 1.0 (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (has to be log(x) of actual predictions) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="full"> Boolean flag. true for logPoissonFull, false for logPoisson </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function logPoisson(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal full As Boolean) As SDVariable
		SDValidation.validateNumerical("logPoisson", "label", label)
		SDValidation.validateNumerical("logPoisson", "predictions", predictions)
		SDValidation.validateNumerical("logPoisson", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.LogPoissonLoss(sd,label, predictions, weights, lossReduce, full)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Log poisson loss: a loss function used for training classifiers.<br>
	  ''' Implements {@code L = exp(c) - z * c} where c is log(predictions) and z is labels.<br>
	  ''' </summary>
	  ''' <param name="label"> Label array. Each value should be 0.0 or 1.0 (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (has to be log(x) of actual predictions) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="full"> Boolean flag. true for logPoissonFull, false for logPoisson </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function logPoisson(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal full As Boolean) As SDVariable
		SDValidation.validateNumerical("logPoisson", "label", label)
		SDValidation.validateNumerical("logPoisson", "predictions", predictions)
		SDValidation.validateNumerical("logPoisson", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.LogPoissonLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, full)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Log poisson loss: a loss function used for training classifiers.<br>
	  ''' Implements {@code L = exp(c) - z * c} where c is log(predictions) and z is labels.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array. Each value should be 0.0 or 1.0 (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (has to be log(x) of actual predictions) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="full"> Boolean flag. true for logPoissonFull, false for logPoisson </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function logPoisson(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal full As Boolean) As SDVariable
		SDValidation.validateNumerical("logPoisson", "label", label)
		SDValidation.validateNumerical("logPoisson", "predictions", predictions)
		SDValidation.validateNumerical("logPoisson", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.LogPoissonLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, full)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Mean pairwise squared error.<br>
	  ''' MPWSE loss calculates the difference between pairs of consecutive elements in the predictions and labels arrays.<br>
	  ''' For example, if predictions = [p0, p1, p2] and labels are [l0, l1, l2] then MPWSE is:<br>
	  ''' {@code [((p0-p1) - (l0-l1))^2 + ((p0-p2) - (l0-l2))^2 + ((p1-p2) - (l1-l2))^2] / 3}<br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used. Must be either null, scalar, or have shape [batchSize] (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <returns> output Loss variable, scalar output (NUMERIC type) </returns>
	  Public Overridable Function meanPairwiseSquaredError(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce) As SDVariable
		SDValidation.validateNumerical("meanPairwiseSquaredError", "label", label)
		SDValidation.validateNumerical("meanPairwiseSquaredError", "predictions", predictions)
		SDValidation.validateNumerical("meanPairwiseSquaredError", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.MeanPairwiseSquaredErrorLoss(sd,label, predictions, weights, lossReduce)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Mean pairwise squared error.<br>
	  ''' MPWSE loss calculates the difference between pairs of consecutive elements in the predictions and labels arrays.<br>
	  ''' For example, if predictions = [p0, p1, p2] and labels are [l0, l1, l2] then MPWSE is:<br>
	  ''' {@code [((p0-p1) - (l0-l1))^2 + ((p0-p2) - (l0-l2))^2 + ((p1-p2) - (l1-l2))^2] / 3}<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used. Must be either null, scalar, or have shape [batchSize] (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <returns> output Loss variable, scalar output (NUMERIC type) </returns>
	  Public Overridable Function meanPairwiseSquaredError(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce) As SDVariable
		SDValidation.validateNumerical("meanPairwiseSquaredError", "label", label)
		SDValidation.validateNumerical("meanPairwiseSquaredError", "predictions", predictions)
		SDValidation.validateNumerical("meanPairwiseSquaredError", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.MeanPairwiseSquaredErrorLoss(sd,label, predictions, weights, lossReduce)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Mean pairwise squared error.<br>
	  ''' MPWSE loss calculates the difference between pairs of consecutive elements in the predictions and labels arrays.<br>
	  ''' For example, if predictions = [p0, p1, p2] and labels are [l0, l1, l2] then MPWSE is:<br>
	  ''' {@code [((p0-p1) - (l0-l1))^2 + ((p0-p2) - (l0-l2))^2 + ((p1-p2) - (l1-l2))^2] / 3}<br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used. Must be either null, scalar, or have shape [batchSize] (NUMERIC type) </param>
	  ''' <returns> output Loss variable, scalar output (NUMERIC type) </returns>
	  Public Overridable Function meanPairwiseSquaredError(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("meanPairwiseSquaredError", "label", label)
		SDValidation.validateNumerical("meanPairwiseSquaredError", "predictions", predictions)
		SDValidation.validateNumerical("meanPairwiseSquaredError", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.MeanPairwiseSquaredErrorLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Mean pairwise squared error.<br>
	  ''' MPWSE loss calculates the difference between pairs of consecutive elements in the predictions and labels arrays.<br>
	  ''' For example, if predictions = [p0, p1, p2] and labels are [l0, l1, l2] then MPWSE is:<br>
	  ''' {@code [((p0-p1) - (l0-l1))^2 + ((p0-p2) - (l0-l2))^2 + ((p1-p2) - (l1-l2))^2] / 3}<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used. Must be either null, scalar, or have shape [batchSize] (NUMERIC type) </param>
	  ''' <returns> output Loss variable, scalar output (NUMERIC type) </returns>
	  Public Overridable Function meanPairwiseSquaredError(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("meanPairwiseSquaredError", "label", label)
		SDValidation.validateNumerical("meanPairwiseSquaredError", "predictions", predictions)
		SDValidation.validateNumerical("meanPairwiseSquaredError", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.MeanPairwiseSquaredErrorLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Mean squared error loss function. Implements {@code (label[i] - prediction[i])^2} - i.e., squared error on a per-element basis.<br>
	  ''' When averaged (using LossReduce#MEAN_BY_WEIGHT or LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT (the default))<br>
	  ''' this is the mean squared error loss function.<br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function meanSquaredError(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce) As SDVariable
		SDValidation.validateNumerical("meanSquaredError", "label", label)
		SDValidation.validateNumerical("meanSquaredError", "predictions", predictions)
		SDValidation.validateNumerical("meanSquaredError", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.MeanSquaredErrorLoss(sd,label, predictions, weights, lossReduce)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Mean squared error loss function. Implements {@code (label[i] - prediction[i])^2} - i.e., squared error on a per-element basis.<br>
	  ''' When averaged (using LossReduce#MEAN_BY_WEIGHT or LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT (the default))<br>
	  ''' this is the mean squared error loss function.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function meanSquaredError(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce) As SDVariable
		SDValidation.validateNumerical("meanSquaredError", "label", label)
		SDValidation.validateNumerical("meanSquaredError", "predictions", predictions)
		SDValidation.validateNumerical("meanSquaredError", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.MeanSquaredErrorLoss(sd,label, predictions, weights, lossReduce)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Mean squared error loss function. Implements {@code (label[i] - prediction[i])^2} - i.e., squared error on a per-element basis.<br>
	  ''' When averaged (using LossReduce#MEAN_BY_WEIGHT or LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT (the default))<br>
	  ''' this is the mean squared error loss function.<br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function meanSquaredError(ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("meanSquaredError", "label", label)
		SDValidation.validateNumerical("meanSquaredError", "predictions", predictions)
		SDValidation.validateNumerical("meanSquaredError", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.MeanSquaredErrorLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Mean squared error loss function. Implements {@code (label[i] - prediction[i])^2} - i.e., squared error on a per-element basis.<br>
	  ''' When averaged (using LossReduce#MEAN_BY_WEIGHT or LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT (the default))<br>
	  ''' this is the mean squared error loss function.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictions"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function meanSquaredError(ByVal name As String, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("meanSquaredError", "label", label)
		SDValidation.validateNumerical("meanSquaredError", "predictions", predictions)
		SDValidation.validateNumerical("meanSquaredError", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.MeanSquaredErrorLoss(sd,label, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sigmoid cross entropy: applies the sigmoid activation function on the input logits (input "pre-sigmoid preductions")<br>
	  ''' and implements the binary cross entropy loss function. This implementation is numerically more stable than using<br>
	  ''' standard (but separate) sigmoid activation function and log loss (binary cross entropy) loss function.<br>
	  ''' Implements:<br>
	  ''' {@code -1/numExamples * sum_i (labels[i] * log(sigmoid(logits[i])) + (1-labels[i]) * log(1-sigmoid(logits[i])))}<br>
	  ''' though this is done in a mathematically equivalent but more numerical stable form.<br>
	  ''' <br>
	  ''' When label smoothing is > 0, the following label smoothing is used:<br>
	  ''' <pre><br>
	  ''' {@code numClasses = labels.size(1);<br>
	  ''' label = (1.0 - labelSmoothing) * label + 0.5 * labelSmoothing}<br>
	  ''' </pre><br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictionLogits"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="labelSmoothing"> Label smoothing value. Default value: 0 </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function sigmoidCrossEntropy(ByVal label As SDVariable, ByVal predictionLogits As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal labelSmoothing As Double) As SDVariable
		SDValidation.validateNumerical("sigmoidCrossEntropy", "label", label)
		SDValidation.validateNumerical("sigmoidCrossEntropy", "predictionLogits", predictionLogits)
		SDValidation.validateNumerical("sigmoidCrossEntropy", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.SigmoidCrossEntropyLoss(sd,label, predictionLogits, weights, lossReduce, labelSmoothing)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Sigmoid cross entropy: applies the sigmoid activation function on the input logits (input "pre-sigmoid preductions")<br>
	  ''' and implements the binary cross entropy loss function. This implementation is numerically more stable than using<br>
	  ''' standard (but separate) sigmoid activation function and log loss (binary cross entropy) loss function.<br>
	  ''' Implements:<br>
	  ''' {@code -1/numExamples * sum_i (labels[i] * log(sigmoid(logits[i])) + (1-labels[i]) * log(1-sigmoid(logits[i])))}<br>
	  ''' though this is done in a mathematically equivalent but more numerical stable form.<br>
	  ''' <br>
	  ''' When label smoothing is > 0, the following label smoothing is used:<br>
	  ''' <pre><br>
	  ''' {@code numClasses = labels.size(1);<br>
	  ''' label = (1.0 - labelSmoothing) * label + 0.5 * labelSmoothing}<br>
	  ''' </pre><br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictionLogits"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="labelSmoothing"> Label smoothing value. Default value: 0 </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function sigmoidCrossEntropy(ByVal name As String, ByVal label As SDVariable, ByVal predictionLogits As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal labelSmoothing As Double) As SDVariable
		SDValidation.validateNumerical("sigmoidCrossEntropy", "label", label)
		SDValidation.validateNumerical("sigmoidCrossEntropy", "predictionLogits", predictionLogits)
		SDValidation.validateNumerical("sigmoidCrossEntropy", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.SigmoidCrossEntropyLoss(sd,label, predictionLogits, weights, lossReduce, labelSmoothing)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sigmoid cross entropy: applies the sigmoid activation function on the input logits (input "pre-sigmoid preductions")<br>
	  ''' and implements the binary cross entropy loss function. This implementation is numerically more stable than using<br>
	  ''' standard (but separate) sigmoid activation function and log loss (binary cross entropy) loss function.<br>
	  ''' Implements:<br>
	  ''' {@code -1/numExamples * sum_i (labels[i] * log(sigmoid(logits[i])) + (1-labels[i]) * log(1-sigmoid(logits[i])))}<br>
	  ''' though this is done in a mathematically equivalent but more numerical stable form.<br>
	  ''' <br>
	  ''' When label smoothing is > 0, the following label smoothing is used:<br>
	  ''' <pre><br>
	  ''' {@code numClasses = labels.size(1);<br>
	  ''' label = (1.0 - labelSmoothing) * label + 0.5 * labelSmoothing}<br>
	  ''' </pre><br>
	  ''' </summary>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictionLogits"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function sigmoidCrossEntropy(ByVal label As SDVariable, ByVal predictionLogits As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("sigmoidCrossEntropy", "label", label)
		SDValidation.validateNumerical("sigmoidCrossEntropy", "predictionLogits", predictionLogits)
		SDValidation.validateNumerical("sigmoidCrossEntropy", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.SigmoidCrossEntropyLoss(sd,label, predictionLogits, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, 0.0)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Sigmoid cross entropy: applies the sigmoid activation function on the input logits (input "pre-sigmoid preductions")<br>
	  ''' and implements the binary cross entropy loss function. This implementation is numerically more stable than using<br>
	  ''' standard (but separate) sigmoid activation function and log loss (binary cross entropy) loss function.<br>
	  ''' Implements:<br>
	  ''' {@code -1/numExamples * sum_i (labels[i] * log(sigmoid(logits[i])) + (1-labels[i]) * log(1-sigmoid(logits[i])))}<br>
	  ''' though this is done in a mathematically equivalent but more numerical stable form.<br>
	  ''' <br>
	  ''' When label smoothing is > 0, the following label smoothing is used:<br>
	  ''' <pre><br>
	  ''' {@code numClasses = labels.size(1);<br>
	  ''' label = (1.0 - labelSmoothing) * label + 0.5 * labelSmoothing}<br>
	  ''' </pre><br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="label"> Label array (NUMERIC type) </param>
	  ''' <param name="predictionLogits"> Predictions array (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function sigmoidCrossEntropy(ByVal name As String, ByVal label As SDVariable, ByVal predictionLogits As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("sigmoidCrossEntropy", "label", label)
		SDValidation.validateNumerical("sigmoidCrossEntropy", "predictionLogits", predictionLogits)
		SDValidation.validateNumerical("sigmoidCrossEntropy", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.SigmoidCrossEntropyLoss(sd,label, predictionLogits, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, 0.0)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Applies the softmax activation function to the input, then implement multi-class cross entropy:<br>
	  ''' {@code -sum_classes label[i] * log(p[c])} where {@code p = softmax(logits)}<br>
	  ''' If LossReduce#NONE is used, returned shape is [numExamples] out for [numExamples, numClasses] predicitons/labels;<br>
	  ''' otherwise, the output is a scalar.<br>
	  ''' <para><br>
	  ''' When label smoothing is > 0, the following label smoothing is used:<br>
	  ''' <pre><br>
	  ''' {@code numClasses = labels.size(1);<br>
	  ''' oneHotLabel = (1.0 - labelSmoothing) * oneHotLabels + labelSmoothing/numClasses}<br>
	  ''' </pre><br>
	  ''' 
	  ''' </para>
	  ''' </summary>
	  ''' <param name="oneHotLabels"> Label array. Should be one-hot per example and same shape as predictions (for example, [mb, nOut]) (NUMERIC type) </param>
	  ''' <param name="logitPredictions"> Predictions array (pre-softmax) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="labelSmoothing"> Label smoothing value. Default value: 0 </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function softmaxCrossEntropy(ByVal oneHotLabels As SDVariable, ByVal logitPredictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal labelSmoothing As Double) As SDVariable
		SDValidation.validateNumerical("softmaxCrossEntropy", "oneHotLabels", oneHotLabels)
		SDValidation.validateNumerical("softmaxCrossEntropy", "logitPredictions", logitPredictions)
		SDValidation.validateNumerical("softmaxCrossEntropy", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.SoftmaxCrossEntropyLoss(sd,oneHotLabels, logitPredictions, weights, lossReduce, labelSmoothing)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Applies the softmax activation function to the input, then implement multi-class cross entropy:<br>
	  ''' {@code -sum_classes label[i] * log(p[c])} where {@code p = softmax(logits)}<br>
	  ''' If LossReduce#NONE is used, returned shape is [numExamples] out for [numExamples, numClasses] predicitons/labels;<br>
	  ''' otherwise, the output is a scalar.<br>
	  ''' <para><br>
	  ''' When label smoothing is > 0, the following label smoothing is used:<br>
	  ''' <pre><br>
	  ''' {@code numClasses = labels.size(1);<br>
	  ''' oneHotLabel = (1.0 - labelSmoothing) * oneHotLabels + labelSmoothing/numClasses}<br>
	  ''' </pre><br>
	  ''' 
	  ''' </para>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="oneHotLabels"> Label array. Should be one-hot per example and same shape as predictions (for example, [mb, nOut]) (NUMERIC type) </param>
	  ''' <param name="logitPredictions"> Predictions array (pre-softmax) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <param name="lossReduce"> Reduction type for the loss. See LossReduce for more details. Default: LossReduce#MEAN_BY_NONZERO_WEIGHT_COUNT </param>
	  ''' <param name="labelSmoothing"> Label smoothing value. Default value: 0 </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function softmaxCrossEntropy(ByVal name As String, ByVal oneHotLabels As SDVariable, ByVal logitPredictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal labelSmoothing As Double) As SDVariable
		SDValidation.validateNumerical("softmaxCrossEntropy", "oneHotLabels", oneHotLabels)
		SDValidation.validateNumerical("softmaxCrossEntropy", "logitPredictions", logitPredictions)
		SDValidation.validateNumerical("softmaxCrossEntropy", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.SoftmaxCrossEntropyLoss(sd,oneHotLabels, logitPredictions, weights, lossReduce, labelSmoothing)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Applies the softmax activation function to the input, then implement multi-class cross entropy:<br>
	  ''' {@code -sum_classes label[i] * log(p[c])} where {@code p = softmax(logits)}<br>
	  ''' If LossReduce#NONE is used, returned shape is [numExamples] out for [numExamples, numClasses] predicitons/labels;<br>
	  ''' otherwise, the output is a scalar.<br>
	  ''' <para><br>
	  ''' When label smoothing is > 0, the following label smoothing is used:<br>
	  ''' <pre><br>
	  ''' {@code numClasses = labels.size(1);<br>
	  ''' oneHotLabel = (1.0 - labelSmoothing) * oneHotLabels + labelSmoothing/numClasses}<br>
	  ''' </pre><br>
	  ''' 
	  ''' </para>
	  ''' </summary>
	  ''' <param name="oneHotLabels"> Label array. Should be one-hot per example and same shape as predictions (for example, [mb, nOut]) (NUMERIC type) </param>
	  ''' <param name="logitPredictions"> Predictions array (pre-softmax) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function softmaxCrossEntropy(ByVal oneHotLabels As SDVariable, ByVal logitPredictions As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("softmaxCrossEntropy", "oneHotLabels", oneHotLabels)
		SDValidation.validateNumerical("softmaxCrossEntropy", "logitPredictions", logitPredictions)
		SDValidation.validateNumerical("softmaxCrossEntropy", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.SoftmaxCrossEntropyLoss(sd,oneHotLabels, logitPredictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, 0.0)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Applies the softmax activation function to the input, then implement multi-class cross entropy:<br>
	  ''' {@code -sum_classes label[i] * log(p[c])} where {@code p = softmax(logits)}<br>
	  ''' If LossReduce#NONE is used, returned shape is [numExamples] out for [numExamples, numClasses] predicitons/labels;<br>
	  ''' otherwise, the output is a scalar.<br>
	  ''' <para><br>
	  ''' When label smoothing is > 0, the following label smoothing is used:<br>
	  ''' <pre><br>
	  ''' {@code numClasses = labels.size(1);<br>
	  ''' oneHotLabel = (1.0 - labelSmoothing) * oneHotLabels + labelSmoothing/numClasses}<br>
	  ''' </pre><br>
	  ''' 
	  ''' </para>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="oneHotLabels"> Label array. Should be one-hot per example and same shape as predictions (for example, [mb, nOut]) (NUMERIC type) </param>
	  ''' <param name="logitPredictions"> Predictions array (pre-softmax) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function softmaxCrossEntropy(ByVal name As String, ByVal oneHotLabels As SDVariable, ByVal logitPredictions As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("softmaxCrossEntropy", "oneHotLabels", oneHotLabels)
		SDValidation.validateNumerical("softmaxCrossEntropy", "logitPredictions", logitPredictions)
		SDValidation.validateNumerical("softmaxCrossEntropy", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.SoftmaxCrossEntropyLoss(sd,oneHotLabels, logitPredictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, 0.0)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' As per softmaxCrossEntropy(String, SDVariable, SDVariable, LossReduce) but the labels variable<br>
	  ''' is represented as an integer array instead of the equivalent one-hot array.<br>
	  ''' i.e., if logits are rank N, then labels have rank N-1<br>
	  ''' </summary>
	  ''' <param name="logits"> Logits array ("pre-softmax activations") (NUMERIC type) </param>
	  ''' <param name="labels"> Labels array. Must be an integer type. (INT type) </param>
	  ''' <returns> output Softmax cross entropy (NUMERIC type) </returns>
	  Public Overridable Function sparseSoftmaxCrossEntropy(ByVal logits As SDVariable, ByVal labels As SDVariable) As SDVariable
		SDValidation.validateNumerical("sparseSoftmaxCrossEntropy", "logits", logits)
		SDValidation.validateInteger("sparseSoftmaxCrossEntropy", "labels", labels)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.SparseSoftmaxCrossEntropyLossWithLogits(sd,logits, labels)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' As per softmaxCrossEntropy(String, SDVariable, SDVariable, LossReduce) but the labels variable<br>
	  ''' is represented as an integer array instead of the equivalent one-hot array.<br>
	  ''' i.e., if logits are rank N, then labels have rank N-1<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="logits"> Logits array ("pre-softmax activations") (NUMERIC type) </param>
	  ''' <param name="labels"> Labels array. Must be an integer type. (INT type) </param>
	  ''' <returns> output Softmax cross entropy (NUMERIC type) </returns>
	  Public Overridable Function sparseSoftmaxCrossEntropy(ByVal name As String, ByVal logits As SDVariable, ByVal labels As SDVariable) As SDVariable
		SDValidation.validateNumerical("sparseSoftmaxCrossEntropy", "logits", logits)
		SDValidation.validateInteger("sparseSoftmaxCrossEntropy", "labels", labels)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.SparseSoftmaxCrossEntropyLossWithLogits(sd,logits, labels)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Weighted cross entropy loss with logits<br>
	  ''' </summary>
	  ''' <param name="targets"> targets array (NUMERIC type) </param>
	  ''' <param name="inputs"> input array (NUMERIC type) </param>
	  ''' <param name="weights"> eights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function weightedCrossEntropyWithLogits(ByVal targets As SDVariable, ByVal inputs As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("weightedCrossEntropyWithLogits", "targets", targets)
		SDValidation.validateNumerical("weightedCrossEntropyWithLogits", "inputs", inputs)
		SDValidation.validateNumerical("weightedCrossEntropyWithLogits", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.WeightedCrossEntropyLoss(sd,targets, inputs, weights)).outputVariable()
		[out].markAsLoss()
		Return [out]
	  End Function

	  ''' <summary>
	  ''' Weighted cross entropy loss with logits<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="targets"> targets array (NUMERIC type) </param>
	  ''' <param name="inputs"> input array (NUMERIC type) </param>
	  ''' <param name="weights"> eights array. May be null. If null, a weight of 1.0 is used (NUMERIC type) </param>
	  ''' <returns> output Loss variable (NUMERIC type) </returns>
	  Public Overridable Function weightedCrossEntropyWithLogits(ByVal name As String, ByVal targets As SDVariable, ByVal inputs As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("weightedCrossEntropyWithLogits", "targets", targets)
		SDValidation.validateNumerical("weightedCrossEntropyWithLogits", "inputs", inputs)
		SDValidation.validateNumerical("weightedCrossEntropyWithLogits", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.loss.WeightedCrossEntropyLoss(sd,targets, inputs, weights)).outputVariable()
		[out].markAsLoss()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function
	End Class

End Namespace