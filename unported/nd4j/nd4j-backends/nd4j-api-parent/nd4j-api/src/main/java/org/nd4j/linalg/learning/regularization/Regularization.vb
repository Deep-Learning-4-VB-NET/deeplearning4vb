Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.nd4j.linalg.learning.regularization


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface Regularization extends java.io.Serializable
	Public Interface Regularization

		''' <summary>
		''' ApplyStep determines how the regularization interacts with the optimization process - i.e., when it is applied
		''' relative to updaters like Adam, Nesterov momentum, SGD, etc.
		''' <br>
		''' <br>
		''' BEFORE_UPDATER: w -= updater(gradient + regularization(p,gradView,lr)) <br>
		''' POST_UPDATER: w -= (updater(gradient) + regularization(p,gradView,lr)) <br>
		''' 
		''' </summary>
		Friend Enum ApplyStep
			BEFORE_UPDATER
			POST_UPDATER
		End Enum

		''' <returns> The step that the regularization should be applied, as defined by <seealso cref="ApplyStep"/> </returns>
		Function applyStep() As ApplyStep

		''' <summary>
		''' Apply the regularization by modifying the gradient array in-place
		''' </summary>
		''' <param name="param">     Input array (usually parameters) </param>
		''' <param name="gradView">  Gradient view array (should be modified/updated). Same shape and type as the input array. </param>
		''' <param name="lr">        Current learning rate </param>
		''' <param name="iteration"> Current network training iteration </param>
		''' <param name="epoch">     Current network training epoch </param>
		Sub apply(ByVal param As INDArray, ByVal gradView As INDArray, ByVal lr As Double, ByVal iteration As Integer, ByVal epoch As Integer)

		''' <summary>
		''' Calculate the loss function score component for the regularization.<br>
		''' For example, in L2 regularization, this would return {@code L = 0.5 * sum_i param[i]^2}<br>
		''' For regularization types that don't have a score component, this method can return 0. However, note that this may
		''' make the regularization type not gradient checkable.
		''' </summary>
		''' <param name="param">     Input array (usually parameters) </param>
		''' <param name="iteration"> Current network training iteration </param>
		''' <param name="epoch">     Current network training epoch </param>
		''' <returns>          Loss function score component based on the input/parameters array </returns>
		Function score(ByVal param As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) As Double

		''' <returns> An independent copy of the regularization instance </returns>
		Function clone() As Regularization

	End Interface

End Namespace