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

Namespace org.nd4j.parameterserver.distributed.enums

	Public Enum ExecutionMode
		''' <summary>
		''' This option assumes data (parameters) are split over multiple hosts
		''' </summary>
		SHARDED

		''' <summary>
		''' This option assumes data stored on multiple shards at the same time
		''' </summary>
		AVERAGING

		''' <summary>
		''' This option means data storage is controlled by application, and out of VoidParameterServer control
		''' </summary>
		MANAGED
	End Enum

End Namespace