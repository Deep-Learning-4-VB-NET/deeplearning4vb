Imports System.Collections.Generic
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.nn.api



	Public Interface Layer
		Inherits Cloneable, Model, Trainable

		Friend Enum Type
			FEED_FORWARD
			RECURRENT
			CONVOLUTIONAL
			CONVOLUTIONAL3D
			SUBSAMPLING
			UPSAMPLING
			RECURSIVE
			MULTILAYER
			NORMALIZATION
		End Enum

		Friend Enum TrainingMode
			TRAIN
			TEST
		End Enum

		''' <summary>
		''' This method sets given CacheMode for current layer
		''' </summary>
		''' <param name="mode"> </param>
		WriteOnly Property CacheMode As CacheMode

		''' <summary>
		''' Calculate the regularization component of the score, for the parameters in this layer<br>
		''' For example, the L1, L2 and/or weight decay components of the loss function<br>
		''' </summary>
		''' <param name="backpropOnlyParams"> If true: calculate regularization score based on backprop params only. If false: calculate
		'''                           based on all params (including pretrain params, if any) </param>
		''' <returns> the regularization score of </returns>
		Function calcRegularizationScore(ByVal backpropOnlyParams As Boolean) As Double

		''' <summary>
		''' Returns the layer type
		''' 
		''' @return
		''' </summary>
		Function type() As Type


		''' <summary>
		''' Calculate the gradient relative to the error in the next layer
		''' </summary>
		''' <param name="epsilon">      w^(L+1)*delta^(L+1). Or, equiv: dC/da, i.e., (dC/dz)*(dz/da) = dC/da, where C
		'''                     is cost function a=sigma(z) is activation. </param>
		''' <param name="workspaceMgr"> Workspace manager </param>
		''' <returns> Pair<Gradient   ,   INDArray> where Gradient is gradient for this layer, INDArray is epsilon (activation gradient)
		''' needed by next layer, but before element-wise multiply by sigmaPrime(z). So for standard feed-forward layer, if this layer is
		''' L, then return.getSecond() == dL/dIn = (w^(L)*(delta^(L))^T)^T. Note that the returned array should be placed in the
		''' <seealso cref="org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD"/> workspace via the workspace manager </returns>
		Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)


		''' <summary>
		''' Perform forward pass and return the activations array with the last set input
		''' </summary>
		''' <param name="training">     training or test mode </param>
		''' <param name="workspaceMgr"> Workspace manager </param>
		''' <returns> the activation (layer output) of the last specified input. Note that the returned array should be placed
		''' in the <seealso cref="org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS"/> workspace via the workspace manager </returns>
		Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray

		''' <summary>
		''' Perform forward pass and return the activations array with the specified input
		''' </summary>
		''' <param name="input">    the input to use </param>
		''' <param name="training"> train or test mode </param>
		''' <param name="mgr">      Workspace manager. </param>
		''' <returns> Activations array. Note that the returned array should be placed in the
		''' <seealso cref="org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS"/> workspace via the workspace manager </returns>
		Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal mgr As LayerWorkspaceMgr) As INDArray

		''' <summary>
		''' Get the iteration listeners for this layer.
		''' </summary>
		Property Listeners As ICollection(Of TrainingListener)


		''' <summary>
		''' Set the <seealso cref="TrainingListener"/>s for this model. If any listeners have previously been set, they will be
		''' replaced by this method
		''' </summary>
		WriteOnly Property Listeners As ICollection(Of TrainingListener)

		''' <summary>
		''' Set the layer index.
		''' </summary>
		Property Index As Integer


		''' <returns> The current iteration count (number of parameter updates) for the layer/network </returns>
		Property IterationCount As Integer

		''' <returns> The current epoch count (number of training epochs passed) for the layer/network </returns>
		Property EpochCount As Integer



		''' <summary>
		''' Set the layer input.
		''' </summary>
		Sub setInput(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)

		''' <summary>
		''' Set current/last input mini-batch size.<br>
		''' Used for score and gradient calculations. Mini batch size may be different from
		''' getInput().size(0) due to reshaping operations - for example, when using RNNs with
		''' DenseLayer and OutputLayer. Called automatically during forward pass.
		''' </summary>
		Property InputMiniBatchSize As Integer


		''' <summary>
		''' Set the mask array. Note: In general, <seealso cref="feedForwardMaskArray(INDArray, MaskState, Integer)"/> should be used in
		''' preference to this.
		''' </summary>
		''' <param name="maskArray"> Mask array to set </param>
		Property MaskArray As INDArray



		''' <summary>
		''' Returns true if the layer can be trained in an unsupervised/pretrain manner (AE, VAE, etc)
		''' </summary>
		''' <returns> true if the layer can be pretrained (using fit(INDArray), false otherwise </returns>
		ReadOnly Property PretrainLayer As Boolean


		Sub clearNoiseWeightParams()

		''' <summary>
		''' A performance optimization: mark whether the layer is allowed to modify its input array in-place. In many cases,
		''' this is totally safe - in others, the input array will be shared by multiple layers, and hence it's not safe to
		''' modify the input array.
		''' This is usually used by ops such as dropout. </summary>
		''' <param name="allow"> If true: the input array is safe to modify. If false: the input array should be copied before it
		'''              is modified (i.e., in-place modifications are un-safe) </param>
		Sub allowInputModification(ByVal allow As Boolean)


		''' <summary>
		''' Feed forward the input mask array, setting in the layer as appropriate. This allows different layers to
		''' handle masks differently - for example, bidirectional RNNs and normal RNNs operate differently with masks (the
		''' former sets activations to 0 outside of the data present region (and keeps the mask active for future layers like
		''' dense layers), whereas normal RNNs don't zero out the activations/errors )instead relying on backpropagated error
		''' arrays to handle the variable length case.<br>
		''' This is also used for example for networks that contain global pooling layers, arbitrary preprocessors, etc.
		''' </summary>
		''' <param name="maskArray">        Mask array to set </param>
		''' <param name="currentMaskState"> Current state of the mask - see <seealso cref="MaskState"/> </param>
		''' <param name="minibatchSize">    Current minibatch size. Needs to be known as it cannot always be inferred from the activations
		'''                         array due to reshaping (such as a DenseLayer within a recurrent neural network) </param>
		''' <returns> New mask array after this layer, along with the new mask state. </returns>
		Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)

		''' <returns> Get the layer helper, if any </returns>
		ReadOnly Property Helper As LayerHelper
	End Interface

End Namespace