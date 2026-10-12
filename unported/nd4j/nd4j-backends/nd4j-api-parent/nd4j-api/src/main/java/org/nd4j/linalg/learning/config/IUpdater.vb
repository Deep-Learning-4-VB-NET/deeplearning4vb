Imports System
Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.learning
Imports ISchedule = org.nd4j.linalg.schedule.ISchedule
Imports JsonAutoDetect = org.nd4j.shade.jackson.annotation.JsonAutoDetect
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

Namespace org.nd4j.linalg.learning.config


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY, getterVisibility = JsonAutoDetect.Visibility.NONE, setterVisibility = JsonAutoDetect.Visibility.NONE) @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface IUpdater extends java.io.Serializable, Cloneable
	Public Interface IUpdater
		Inherits ICloneable

		''' <summary>
		''' Determine the updater state size for the given number of parameters. Usually a integer multiple (0,1 or 2)
		''' times the number of parameters in a layer.
		''' </summary>
		''' <param name="numParams"> Number of parameters </param>
		''' <returns> Updater state size for the given number of parameters </returns>
		Function stateSize(ByVal numParams As Long) As Long

		''' <summary>
		''' Create a new gradient updater
		''' </summary>
		''' <param name="viewArray">           The updater state size view away </param>
		''' <param name="initializeViewArray"> If true: initialise the updater state
		''' @return </param>
		Function instantiate(ByVal viewArray As INDArray, ByVal initializeViewArray As Boolean) As GradientUpdater

		Function instantiate(ByVal updaterState As IDictionary(Of String, INDArray), ByVal initializeStateArrays As Boolean) As GradientUpdater

		Function Equals(ByVal updater As Object) As Boolean

		''' <summary>
		''' Clone the updater
		''' </summary>
		Function clone() As IUpdater

		''' <summary>
		''' Get the learning rate - if any - for the updater, at the specified iteration and epoch.
		''' Note that if no learning rate is applicable (AdaDelta, NoOp updaters etc) then Double.NaN should
		''' be return
		''' </summary>
		''' <param name="iteration"> Iteration at which to get the learning rate </param>
		''' <param name="epoch">     Epoch at which to get the learning rate </param>
		''' <returns>          Learning rate, or Double.NaN if no learning rate is applicable for this updater </returns>
		Function getLearningRate(ByVal iteration As Integer, ByVal epoch As Integer) As Double

		''' <returns> True if the updater has a learning rate hyperparameter, false otherwise </returns>
		Function hasLearningRate() As Boolean

		''' <summary>
		''' Set the learning rate and schedule. Note: may throw an exception if <seealso cref="hasLearningRate()"/> returns false. </summary>
		''' <param name="lr">         Learning rate to set (typically not used if LR schedule is non-null) </param>
		''' <param name="lrSchedule"> Learning rate schedule to set (may be null) </param>
		Sub setLrAndSchedule(ByVal lr As Double, ByVal lrSchedule As ISchedule)



	End Interface

End Namespace