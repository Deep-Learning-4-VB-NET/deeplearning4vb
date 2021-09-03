Imports org.deeplearning4j.rl4j.learning

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

Namespace org.deeplearning4j.rl4j.util


	Public Interface IDataManager

		ReadOnly Property SaveData As Boolean
		ReadOnly Property VideoDir As String
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void appendStat(StatEntry statEntry) throws java.io.IOException;
		Sub appendStat(ByVal statEntry As StatEntry)
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void writeInfo(org.deeplearning4j.rl4j.learning.ILearning iLearning) throws java.io.IOException;
		Sub writeInfo(ByVal iLearning As ILearning)
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void save(org.deeplearning4j.rl4j.learning.ILearning learning) throws java.io.IOException;
		Sub save(ByVal learning As ILearning)

		'In order for jackson to serialize StatEntry
		'please use Lombok @Value (see QLStatEntry)
		Friend Interface StatEntry
			ReadOnly Property EpochCounter As Integer

			ReadOnly Property StepCounter As Integer

			ReadOnly Property Reward As Double
		End Interface
	End Interface

End Namespace