﻿Imports System

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

Namespace org.nd4j.jita.allocator.enums

	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	<Obsolete>
	Public Enum SyncState
		UNDEFINED ' state is unknown
		DESYNC ' memory is desync
		SYNC ' memory is sync
		[PARTIAL] ' memory is accessed using shapes, so patial sync/desync is possible
	End Enum

End Namespace