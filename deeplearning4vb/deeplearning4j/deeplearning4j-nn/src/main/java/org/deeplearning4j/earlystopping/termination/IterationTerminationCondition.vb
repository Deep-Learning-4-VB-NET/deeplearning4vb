Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
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

Namespace org.deeplearning4j.earlystopping.termination


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") @JsonInclude(JsonInclude.Include.NON_NULL) public interface IterationTerminationCondition extends java.io.Serializable
	Public Interface IterationTerminationCondition

		''' <summary>
		''' Initialize the iteration termination condition (sometimes a no-op) </summary>
		Sub initialize()

		''' <summary>
		''' Should early stopping training terminate at this iteration, based on the score for the last iteration?
		''' return true if training should be terminated immediately, or false otherwise </summary>
		''' <param name="lastMiniBatchScore"> Score of the last minibatch </param>
		''' <returns> whether to terminate or not </returns>
		Function terminate(ByVal lastMiniBatchScore As Double) As Boolean

	End Interface

End Namespace