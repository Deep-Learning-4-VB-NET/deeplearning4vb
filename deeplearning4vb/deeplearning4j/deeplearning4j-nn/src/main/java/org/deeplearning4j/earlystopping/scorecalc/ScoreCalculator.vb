Imports Model = org.deeplearning4j.nn.api.Model
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonSubTypes = org.nd4j.shade.jackson.annotation.JsonSubTypes
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

Namespace org.deeplearning4j.earlystopping.scorecalc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") @JsonInclude(JsonInclude.Include.NON_NULL) @JsonSubTypes(value = { @JsonSubTypes.Type(value = DataSetLossCalculator.class, name = "BestScoreEpochTerminationCondition"), @JsonSubTypes.Type(value = DataSetLossCalculatorCG.class, name = "MaxEpochsTerminationCondition")}) public interface ScoreCalculator<T extends org.deeplearning4j.nn.api.Model> extends java.io.Serializable
	Public Interface ScoreCalculator(Of T As org.deeplearning4j.nn.api.Model)

		''' <summary>
		''' Calculate the score for the given MultiLayerNetwork </summary>
		Function calculateScore(ByVal network As T) As Double

		''' <returns> If true: the score should be minimized. If false: the score should be maximized. </returns>
		Function minimizeScore() As Boolean
	End Interface

End Namespace