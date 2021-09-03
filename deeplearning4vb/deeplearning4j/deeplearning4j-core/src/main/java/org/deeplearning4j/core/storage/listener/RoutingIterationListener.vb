Imports System
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports StatsStorageRouterProvider = org.deeplearning4j.core.storage.StatsStorageRouterProvider
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener

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

Namespace org.deeplearning4j.core.storage.listener


	Public Interface RoutingIterationListener
		Inherits TrainingListener, ICloneable

		Property StorageRouter As StatsStorageRouter


		Property WorkerID As String


		Property SessionID As String


		Function clone() As RoutingIterationListener

	End Interface

End Namespace