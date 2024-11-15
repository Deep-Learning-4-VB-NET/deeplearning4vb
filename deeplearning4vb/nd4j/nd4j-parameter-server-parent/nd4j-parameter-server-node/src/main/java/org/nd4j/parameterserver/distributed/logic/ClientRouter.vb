﻿Imports System
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
Imports VoidMessage = org.nd4j.parameterserver.distributed.messages.VoidMessage
Imports Transport = org.nd4j.parameterserver.distributed.transport.Transport

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

Namespace org.nd4j.parameterserver.distributed.logic

	<Obsolete>
	Public Interface ClientRouter

		Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal transport As Transport)

		Function assignTarget(ByVal message As TrainingMessage) As Integer

		Function assignTarget(ByVal message As VoidMessage) As Integer

		WriteOnly Property Originator As VoidMessage
	End Interface

End Namespace