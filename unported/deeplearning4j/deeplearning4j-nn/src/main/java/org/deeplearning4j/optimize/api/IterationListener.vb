Imports System
Imports Model = org.deeplearning4j.nn.api.Model

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

Namespace org.deeplearning4j.optimize.api



	<Obsolete, Serializable>
	Public MustInherit Class IterationListener
		Inherits BaseTrainingListener

		''' <summary>
		''' Event listener for each iteration </summary>
		''' <param name="iteration"> the iteration </param>
		''' <param name="model"> the model iterating </param>
		Public Overrides MustOverride Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)

	End Class

End Namespace