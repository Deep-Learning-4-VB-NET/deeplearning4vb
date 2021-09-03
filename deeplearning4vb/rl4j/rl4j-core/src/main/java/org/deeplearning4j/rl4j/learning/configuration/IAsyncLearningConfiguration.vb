﻿'
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

Namespace org.deeplearning4j.rl4j.learning.configuration

	Public Interface IAsyncLearningConfiguration
		Inherits ILearningConfiguration

		ReadOnly Property NumThreads As Integer

		''' <summary>
		''' The number of steps to collect for each worker thread between each global update
		''' </summary>
		ReadOnly Property NStep As Integer

		''' <summary>
		''' The frequency of worker thread gradient updates to perform a copy of the current working network to the target network
		''' </summary>
		ReadOnly Property LearnerUpdateFrequency As Integer

		ReadOnly Property MaxStep As Integer
	End Interface

End Namespace