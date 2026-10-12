Imports System
Imports lombok
Imports PretrainParamInitializer = org.deeplearning4j.nn.params.PretrainParamInitializer
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties

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

Namespace org.deeplearning4j.nn.conf.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true, exclude = {"pretrain"}) @EqualsAndHashCode(callSuper = true, exclude = {"pretrain"}) @JsonIgnoreProperties("pretrain") public abstract class BasePretrainNetwork extends FeedForwardLayer
	<Serializable>
	Public MustInherit Class BasePretrainNetwork
		Inherits FeedForwardLayer

		Protected Friend lossFunction As LossFunctions.LossFunction
		Protected Friend visibleBiasInit As Double

		Public Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.lossFunction = builder.lossFunction
			Me.visibleBiasInit = builder.visibleBiasInit

		End Sub

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return PretrainParamInitializer.VISIBLE_BIAS_KEY.Equals(paramName)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static abstract class Builder<T extends Builder<T>> extends FeedForwardLayer.Builder<T>
		Public MustInherit Class Builder(Of T As Builder(Of T))
			Inherits FeedForwardLayer.Builder(Of T)

'JAVA TO VB CONVERTER NOTE: The field lossFunction was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend lossFunction_Conflict As LossFunctions.LossFunction = LossFunctions.LossFunction.RECONSTRUCTION_CROSSENTROPY

'JAVA TO VB CONVERTER NOTE: The field visibleBiasInit was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend visibleBiasInit_Conflict As Double = 0.0

			Public Sub New()
			End Sub

'JAVA TO VB CONVERTER NOTE: The parameter lossFunction was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lossFunction(ByVal lossFunction_Conflict As LossFunctions.LossFunction) As T
				Me.setLossFunction(lossFunction_Conflict)
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter visibleBiasInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function visibleBiasInit(ByVal visibleBiasInit_Conflict As Double) As T
				Me.setVisibleBiasInit(visibleBiasInit_Conflict)
				Return CType(Me, T)
			End Function

		End Class
	End Class

End Namespace